"""Algorithm for getting a random initial path"""
from enum import Enum
import random
from typing import List, Set, Tuple

class DeadEnd(Exception):
    """
    Raised when the algorithm reaches a dead end
    """

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


def single_step(path: List[Tuple[int, int]], banned: Set[Direction], x: int, y: int, x_max: int, y_max: int) -> (Direction, int, int):
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
        if (new_x, new_y) in path:
            banned.add(direction)
            continue

        # OK
        return direction, new_x, new_y

def get_path(x_max: int, y_max: int) -> List[Tuple[int, int]]:
    x, y = int(random.randrange(x_max)), int(random.randrange(y_max))
    path = []
    last_direction = Direction.Null
    try:
        while True:
            path.append((x, y))
            last_direction, x, y = single_step(path, { last_direction.opposite() }, x, y, x_max, y_max)
    except DeadEnd:
        return path
