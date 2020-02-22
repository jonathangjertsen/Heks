"""Algorithm for getting a random initial path"""
from enum import Enum
import random
from typing import List, Set, Tuple

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
        return random.choice([Direction.North, Direction.South, Direction.East, Direction.West])

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

def can_branch(coord, path):
    return

def add_branches(path, max_x, max_y):
    visited = set(path)
    branches = []
    while True:
        neighbours = get_all_neighbours(visited, max_x, max_y)
        if not any(neighbours.values()):
            return branches
        start = random.choice(list(neighbours.keys()))
        branch = get_path(*start, max_x, max_y, visited)
        visited |= set(branch)
        branches.append(branch)
