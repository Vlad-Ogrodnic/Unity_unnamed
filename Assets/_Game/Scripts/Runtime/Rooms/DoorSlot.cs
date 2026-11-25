using System.Collections.Generic;
using UnityEngine;

namespace Game.Rooms
{
    public enum DoorSide
    {
        North,
        East,
        South,
        West
    }

    /// <summary>One possible door: a side of one of the room's cells. Every outer cell side has one.</summary>
    public readonly struct DoorSlot
    {
        /// <summary>Which of the room's cells (0,0 = south-west).</summary>
        public readonly Vector2Int Cell;
        public readonly DoorSide Side;

        public DoorSlot(Vector2Int cell, DoorSide side)
        {
            Cell = cell;
            Side = side;
        }

        public static Vector2Int Offset(DoorSide side)
        {
            switch (side)
            {
                case DoorSide.North: return Vector2Int.up;
                case DoorSide.East: return Vector2Int.right;
                case DoorSide.South: return Vector2Int.down;
                default: return Vector2Int.left;
            }
        }

        public static DoorSide Opposite(DoorSide side) => (DoorSide)(((int)side + 2) % 4);

        /// <summary>All door slots of a room covering <paramref name="cells"/>.</summary>
        public static IEnumerable<DoorSlot> All(Vector2Int cells)
        {
            for (int x = 0; x < cells.x; x++)
            {
                yield return new DoorSlot(new Vector2Int(x, cells.y - 1), DoorSide.North);
                yield return new DoorSlot(new Vector2Int(x, 0), DoorSide.South);
            }
            for (int y = 0; y < cells.y; y++)
            {
                yield return new DoorSlot(new Vector2Int(cells.x - 1, y), DoorSide.East);
                yield return new DoorSlot(new Vector2Int(0, y), DoorSide.West);
            }
        }

        /// <summary>The interior tiles right inside this door (DoorWidth of them), as (x, z).</summary>
        public IEnumerable<Vector2Int> InsideTiles(Vector2Int interiorSize)
        {
            for (int i = 0; i < RoomGrid.DoorWidth; i++)
            {
                switch (Side)
                {
                    case DoorSide.North:
                        yield return new Vector2Int(RoomGrid.DoorStart(Cell.x, RoomGrid.CellSize.x) + i, interiorSize.y - 1);
                        break;
                    case DoorSide.South:
                        yield return new Vector2Int(RoomGrid.DoorStart(Cell.x, RoomGrid.CellSize.x) + i, 0);
                        break;
                    case DoorSide.East:
                        yield return new Vector2Int(interiorSize.x - 1, RoomGrid.DoorStart(Cell.y, RoomGrid.CellSize.y) + i);
                        break;
                    default:
                        yield return new Vector2Int(0, RoomGrid.DoorStart(Cell.y, RoomGrid.CellSize.y) + i);
                        break;
                }
            }
        }
    }
}
