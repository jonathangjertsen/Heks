"""Algorithm for getting a random initial path"""
from collections import defaultdict
from enum import Enum
import random
from typing import List, Set, Tuple

from shapely.ops import linemerge, polygonize

class Direction(Enum):
    """
    Represents the cardinal directions
    The value of each enum is the number of steps to go along x and y
    """
    North = (0, 1)
    East = (1, 0)
    South = (0, -1)
    West = (-1, 0)
    Null = (0, 0)

    @classmethod
    def random(cls) -> 'Direction':
        return random.choice(cls.all())

    @classmethod
    def all(self) -> List['Direction']:
        return [Direction.North, Direction.South, Direction.East, Direction.West]

    def opposite(self) -> 'Direction':
        if self == Direction.North:
            return Direction.South
        elif self == Direction.East:
            return Direction.West
        elif self == Direction.South:
            return Direction.North
        elif self == Direction.West:
            return Direction.East
        else:
            return Direction.Null

    def rotate_clockwise(self) -> 'Direction':
        if self == Direction.North:
            return Direction.East
        elif self == Direction.East:
            return Direction.South
        elif self == Direction.South:
            return Direction.West
        elif self == Direction.West:
            return Direction.North
        else:
            return Direction.Null

    def rotate_counter_clockwise(self) -> 'Direction':
        if self == Direction.North:
            return Direction.West
        elif self == Direction.West:
            return Direction.South
        elif self == Direction.South:
            return Direction.East
        elif self == Direction.East:
            return Direction.North
        else:
            return Direction.Null

class DeadEnd(Exception):
    """
    Raised when the algorithm reaches a dead end
    """

def single_step(visited: Set[Tuple[int, int]], banned: Set[Direction], x: int, y: int, x_max: int, y_max: int) -> (Direction, int, int):
    """Returns the direction and new (x,y) coordinate of a random single step, raises DeadEnd if none is possible"""
    while True:
        # All directions explored, none work
        if len(banned) == 4:
            raise DeadEnd()

        # Try one
        direction = Direction.random()
        if direction in banned:
            continue
        new_x, new_y = x + direction.value[0], y + direction.value[1]

        # Check against world edges
        if not (0 <= new_x < x_max):
            banned.add(direction)
            continue

        if not (0 <= new_y < y_max):
            banned.add(direction)
            continue

        # Check if it's in the path
        if (new_x, new_y) in visited:
            banned.add(direction)
            continue

        # OK
        return direction, new_x, new_y

def get_path(x: int, y: int, x_max: int, y_max: int, visited=None) -> List[Tuple[int, int]]:
    path = []
    if visited is None:
        visited = set()
    last_direction = Direction.Null
    try:
        while True:
            path.append((x, y))
            visited.add((x, y))
            last_direction, x, y = single_step(visited, { last_direction.opposite() }, x, y, x_max, y_max)
    except DeadEnd:
        return path

def get_initial_path(x_max: int, y_max: int, min_frac: float, max_frac: float) -> List[Tuple[int, int]]:
    assert 0 <= min_frac < max_frac <= 1
    area = x_max * y_max
    path = None

    while path is None or not (area * min_frac <= len(path) <= area * max_frac):
        path = get_path(int(random.randrange(x_max)), int(random.randrange(y_max)), x_max, y_max)
    return path

def get_neighbours(coord, path, max_x, max_y):
    result = set()
    x, y = coord
    if 0 < x:
        if 0 < y:
            result.add((x-1, y-1))
        if y < max_y - 1:
            result.add((x-1, y+1))
    if x < max_x - 1:
        if 0 < y:
            result.add((x+1, y-1))
        if y < max_y - 1:
            result.add((x+1, y+1))
    result -= { move for move in result if move in path }
    return result

def get_all_neighbours(path, max_x, max_y):
    neighbours = {
        coord: get_neighbours(coord, path, max_x, max_y)
        for coord in path
    }
    return neighbours

def add_branches(path, max_x, max_y, max_branches=None):
    visited = set(path)
    branches = []
    while max_branches is None or len(branches) < max_branches:
        neighbours = get_all_neighbours(visited, max_x, max_y)
        if not any(neighbours.values()):
            return branches
        start = random.choice(list(neighbours.keys()))
        branch = get_path(*start, max_x, max_y, visited)
        visited |= set(branch)
        branches.append(branch)
    return branches

def get_edges(paths):
    return {(start, stop) for path in paths for start, stop in zip(path[:-1], path[1:])}

def get_wall(left, right):
    if abs(left[0] - right[0]) < 0.001:
        # vertical
        if abs(left[0]) < 0.001:
            return Direction.East
        else:
            return Direction.West
    else:
        # horizontal
        if (abs(left[1])) < 0.001:
            return Direction.South
        else:
            return Direction.North

def get_openings(edges, min_width=0.2, max_width=0.8):
    all_openings = defaultdict(list)
    max_nonwidth = 1 - max_width
    for (start_x, start_y), (stop_x, stop_y) in edges:
        openings = (
            max_nonwidth + random.random() * (0.5-max_nonwidth-min_width/2),
            max_nonwidth - random.random() * (0.5-max_nonwidth-min_width/2),
        )
        openings_rev = (1 - openings[0], 1 - openings[1])
        openings_start = all_openings[(start_x, start_y)]
        openings_stop = all_openings[(stop_x, stop_y)]
        if start_x < stop_x:
            openings_start.append((Direction.East, openings))
            openings_stop.append((Direction.West, openings_rev))
        elif stop_x < start_x:
            openings_start.append((Direction.West, openings))
            openings_stop.append((Direction.East, openings_rev))
        elif start_y < stop_y:
            openings_start.append((Direction.North, openings))
            openings_stop.append((Direction.South, openings_rev))
        elif stop_y < start_y:
            openings_start.append((Direction.South, openings))
            openings_stop.append((Direction.North, openings_rev))
        else:
            raise Exception
    return all_openings

def rotate_from_north(direction, path):
    if direction == Direction.North:
        return path
    elif direction == Direction.East:
        return [(y, 1 - x) for x, y in path]
    elif direction == Direction.South:
        return [(1 - x, 1 - y) for x, y in path]
    elif direction == Direction.West:
        return [(1 - y, x) for x, y in path]
    else:
        raise ValueError(path)

def get_oriented_openings(openings):
    return [
        rotate_from_north(direction, [(right, 1), (left, 1)])
        for direction, (right, left)
        in openings
    ]

def orientation(p, q, r):
    return (q[1] - p[1]) * (r[0] - q[0]) - (q[0] - p[0]) * (r[1] - q[1]) > 0

def intersect(p1, p2, q1, q2):
    return orientation(p1, q1, p2) == orientation(p2, q2, p1) and orientation(p1, q1, q2) == orientation(p2, q2, q1)

def get_room_with_one_wall(left, right):
    wall = get_wall(left, right)
    left_x, left_y = left
    right_x, right_y = right
    if wall == Direction.North:
        return [
            [(left_x, 1), (left_x, 1 - right_x),],
            [(left_x, 1 - right_x), (right_x, 1 - right_x)],
            [(right_x, 1 - right_x), (right_x, 1)]
        ]
    elif wall == Direction.South:
        return [
            [(left_x, 0), (left_x, 1-right_x),],
            [(left_x, 1-right_x), (right_x, 1-right_x)],
            [(right_x, 1-right_x), (right_x, 0)]
        ]
    elif wall == Direction.West:
        return [
            [(1, left_y), (1-right_y, left_y),],
            [(1-right_y, left_y), (1-right_y, right_y),],
            [(1-right_y, right_y), (1, right_y),]
        ]
    elif wall == Direction.East:
        return [
            [(0, left_y), (right_y, left_y),],
            [(right_y, left_y), (right_y, right_y),],
            [(right_y, right_y), (0, right_y),]
        ]
    else:
        raise ValueError(wall)

def get_tunnel(start, stop):
    start_left, start_right = start
    stop_left, stop_right = stop
    if not intersect(start_left, stop_left, start_right, stop_right):
        return [
            (start_left, stop_left),
            (start_right, stop_right)
        ]
    else:
        return [
            (start_left, stop_right),
            (start_right, stop_left)
        ]

def get_fork(a, b, c):
    (a_left, a_right), (b_left, b_right), (c_left, c_right) = a, b, c
    segments = []
    if not intersect(a_left, b_left, a_right, b_right):
        if intersect(a_left, c_left, a_right, b_right):
            segments.append((a_left, b_left))
            if intersect(a_left, c_left, a_right, c_right):
                segments.append((a_right, c_left))
                segments.append((b_right, c_right))
            else:
                segments.append((a_right, c_right))
                segments.append((b_right, c_left))
        else:
            if not intersect(a_left, c_left, a_right, c_right):
                segments.append((a_left, c_left))
                if intersect(a_left, b_left, a_right, b_right):
                    segments.append((a_right, b_left))
                    segments.append((c_right, b_right))
                else:
                    segments.append((a_right, b_right))
                    segments.append((c_right, b_left))
            else:
                segments.append((a_left, c_right))
                segments.append((a_right, b_right))
                segments.append((b_left, c_left))
        return segments
    else:
        return get_fork(a, c, b)

def get_crossing(oriented_openings):
    walls = {}
    for opening in oriented_openings:
        walls[get_wall(*opening)] = opening
    return [
        [min(walls[Direction.North]), max(walls[Direction.East])],
        [min(walls[Direction.East]), min(walls[Direction.South])],
        [max(walls[Direction.South]), min(walls[Direction.West])],
        [max(walls[Direction.West]), max(walls[Direction.North])],
    ]

def get_connections(oriented_openings):
    n_openings = len(oriented_openings)
    if n_openings == 1:
        return get_room_with_one_wall(*oriented_openings[0])
    elif n_openings == 2:
        return get_tunnel(*oriented_openings)
    elif n_openings == 3:
        return get_fork(*oriented_openings)
    elif n_openings == 4:
        return get_crossing(oriented_openings)
    else:
        raise ValueError(oriented_openings)

def _get_level(
    cells_x=10,
    cells_y=6,
    critical_path_min_frac=0.2,
    critical_path_max_frac=0.8,
    scale=100,
    pre_simplify=0.1,
    post_simplify=0.1,
    buffer=0.2,
    max_branches=None,
):
    initial_path = get_initial_path(cells_x, cells_y, critical_path_min_frac, critical_path_max_frac)
    geometry = linemerge([
        (
            (int((x_c + x_cstart) * scale), int((y_c + y_cstart) * scale)),
            (int((x_c + x_cstop) * scale), int((y_c + y_cstop) * scale)),
        )
        for (x_c, y_c), opening
        in get_openings(
            get_edges(
                [initial_path] + add_branches(
                    initial_path,
                    cells_x,
                    cells_y,
                    max_branches,
                )
            )
        ).items()
        for (x_cstart, y_cstart), (x_cstop, y_cstop)
        in get_connections(
            get_oriented_openings(
                opening
            )
        )
    ]).simplify(pre_simplify*scale, preserve_topology=False).buffer(buffer*scale)
    return geometry.exterior.xy

def get_level(*args, **kwargs):
    max_tries = 100
    tries = 0
    while True:
        try:
            return _get_level(*args, **kwargs)
        except:
            tries += 1
            if tries > max_tries:
                raise Exception("Failed to make a level too many times")
