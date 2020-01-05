import argparse
from collections import namedtuple
from pathlib import Path
import re

import networkx as nx

alphabet = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ\""

def get_scripts_dir():
    here_dir = Path(__file__).absolute()
    return here_dir.parent.parent.parent / "Assets" / "Scripts"

def get_directories():
    scripts_top = get_scripts_dir()
    paths_parents = []
    for i, directory in enumerate(scripts_top.glob("**")):
        parts = directory.relative_to(scripts_top).parts
        if parts:
            paths_parents.append((parts, parts[:-1]))

    nodes = {}
    for part, parent in paths_parents:
        nodes[part] = { 'path': part, 'children': [] }

    forest = []
    for part, parent in paths_parents:
        node = nodes[part]
        if not parent:
            forest.append(node)
        else:
            parent_node = nodes[parent]
            children = parent_node['children']
            children.append(node)
    return forest

def node_to_path(node):
    return Path(*node["path"])

def directory_iter(scripts_dir, directories, func, *args):
    scripts = list(scripts_dir.glob("*.cs"))
    for file in scripts:
        is_cs = str(file).endswith(".cs")
        if is_cs:
            func(file, *args)
    for directory in directories:
        directory_iter(scripts_dir / directory["path"][-1], directory["children"], func, *args)

Class_ = namedtuple("Class_", "name base implements modifiers file interface")

def get_classes():
    def parse_cs(file, classes):
        with open(file) as f:
            for line in f.readlines():
                if "public" in line and ("class" in line or "interface" in line):
                    interface = "interface" in line
                    search_kw = "interface" if interface else "class"
                    line = re.sub(r'[^\x00-\x7f]',r'', line)
                    modifiers, remainder = line.split("{} ".format(search_kw))
                    modifiers = modifiers.strip().split()
                    classname, *bases_and_wheres = remainder.split(":")
                    classname = classname.strip().split("<")[0]
                    if bases_and_wheres:
                        bases = bases_and_wheres[0]
                        bases = [base.strip() for base in bases.split(",") if base.strip()]
                        for i, base in enumerate(bases):
                            if " where " in base:
                                base, _ = base.split(" where ")
                                bases[i] = base
                    else:
                        bases = []
                    base = None
                    interfaces = []
                    if bases:
                        bases0 = bases[0].split("<")[0]
                        if bases0.startswith("I"):
                            interfaces.append(bases0)
                        else:
                            base = bases0
                    interfaces.extend(b.split("<")[0] for b in bases[1:])
                    class_ = Class_(name=classname, base=base, implements=interfaces, modifiers=modifiers, file=file, interface=interface)
                    classes.append(class_)

    classes = []
    directory_iter(get_scripts_dir(), get_directories(), parse_cs, classes)
    return classes

def make_graph(classes, include_dependencies=True):
    graph = nx.DiGraph()
    for class_ in classes:
        graph.add_node(class_.name, is_interface=class_.interface, is_static="static" in class_.modifiers)
    for class_ in classes:
        if class_.base is not None:
            graph.add_edge(class_.name, class_.base, attr="__bold__")
        for interface in class_.implements:
            graph.add_edge(class_.name, interface, attr="__dashed__")
    if include_dependencies:
        for class_ in classes:
            with open(class_.file) as file:
                for line in file.readlines():
                    if "// docgen-skip" in line:
                        continue
                    for class__ in classes:
                        if class_ == class__:
                            continue
                        if class__.name in class_.implements:
                            continue
                        if class__.name == class_.base:
                            continue
                        if "I" + class__.name == class_.name:
                            continue
                        if class__.name in line:
                            betweens = line.split(class__.name)
                            if "//" in betweens[0]:
                                continue
                            nop = False
                            for between in betweens[1:]:
                                if between[0] in alphabet:
                                    nop = True
                                    break
                            for between in betweens[:-1]:
                                if between[-1] in alphabet:
                                    nop = True
                                    break
                            if nop:
                                continue
                            graph.add_edge(class_.name, class__.name)
    return graph

def build_subgraph(sources, include_descendants: bool, include_ancestors: bool):
    nodes = list(sources)
    if include_descendants:
        for source in sources:
            nodes.extend(nx.descendants(graph, source))
    if include_ancestors:
        for source in sources:
            nodes.extend(nx.ancestors(graph, source))
    subgraph = graph.subgraph(nodes)
    return subgraph

def get_pydot(graph: nx.DiGraph) -> str:
    pd = str(nx.nx_pydot.to_pydot(graph))
    text = pd.replace(
        "attr=__bold__", 'style="bold"').replace(
        "attr=__dashed__", 'style="dashed"').replace(
        "is_interface=True, is_static=False", 'style="dashed"').replace(
        "is_interface=False, is_static=False", '').replace(
        "is_interface=True, is_static=True", 'style="filled,dashed" fillcolor="gray90"').replace(
        "is_interface=False, is_static=True", 'style="filled" fillcolor="gray90"'
    )

    return text

if __name__ == "__main__":
    parser = argparse.ArgumentParser(description="Get dependency graph as pydot")
    parser.add_argument("-r", dest="root", help="Root class(es)", nargs="+")
    parser.add_argument("-d", dest="descendants", action="store_true", help="Set flag to include descendants")
    parser.add_argument("-a", dest="ancestors", action="store_true", help="Set flag to include ancestors")
    args = parser.parse_args()

    classes = get_classes()
    graph = make_graph(classes)
    subgraph = build_subgraph(args.root, include_ancestors=args.ancestors, include_descendants=args.descendants)
    text = get_pydot(subgraph)

    print(text)