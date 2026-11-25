using System.Collections.Generic;
using UnityEngine;

namespace Game.Rooms
{
    /// <summary>
    /// Rules every room layout must follow so any door can be used and entering is fair:
    /// door approaches are clear, every walkable tile is reachable, enemies don't start next to a door,
    /// and only implemented tiles are used.
    /// </summary>
    public static class RoomLayoutValidator
    {
        /// <summary>Tiles in front of a door (straight in) that must be plain floor.</summary>
        public const int DoorApproachDepth = 2;

        /// <summary>Enemies must start at least this many tiles (Chebyshev) from any door tile.</summary>
        public const int EnemyDoorDistance = 3;

        public static List<string> Validate(RoomLayout layout)
        {
            var errors = new List<string>();
            Vector2Int size = layout.Size;
            var doorTiles = new List<Vector2Int>();

            foreach (DoorSlot slot in DoorSlot.All(layout.Cells))
            {
                Vector2Int inward = -DoorSlot.Offset(slot.Side);
                foreach (Vector2Int tile in slot.InsideTiles(size))
                {
                    doorTiles.Add(tile);
                    for (int d = 0; d < DoorApproachDepth; d++)
                    {
                        Vector2Int t = tile + inward * d;
                        if (layout[t.x, t.y] != RoomTile.Floor)
                            errors.Add($"{layout.Name}: door approach blocked at {Describe(layout, t)} ({slot.Side} door of cell {slot.Cell}).");
                    }
                }
            }

            for (int x = 0; x < size.x; x++)
            {
                for (int z = 0; z < size.y; z++)
                {
                    RoomTile tile = layout[x, z];
                    if (tile == RoomTile.Destructible)
                        errors.Add($"{layout.Name}: destructible obstacle at {Describe(layout, new Vector2Int(x, z))} isn't supported yet (bombs undecided).");

                    if (IsEnemy(tile))
                    {
                        foreach (Vector2Int door in doorTiles)
                        {
                            if (Mathf.Max(Mathf.Abs(door.x - x), Mathf.Abs(door.y - z)) < EnemyDoorDistance)
                            {
                                errors.Add($"{layout.Name}: enemy at {Describe(layout, new Vector2Int(x, z))} is closer than {EnemyDoorDistance} tiles to a door.");
                                break;
                            }
                        }
                    }
                }
            }

            CheckReachability(layout, doorTiles[0], errors);
            return errors;
        }

        public static bool IsEnemy(RoomTile tile) =>
            tile == RoomTile.Chaser || tile == RoomTile.Shooter || tile == RoomTile.Charger;

        private static bool IsWalkable(RoomTile tile) => tile != RoomTile.Obstacle && tile != RoomTile.Destructible;

        private static void CheckReachability(RoomLayout layout, Vector2Int start, List<string> errors)
        {
            Vector2Int size = layout.Size;
            var seen = new bool[size.x, size.y];
            var queue = new Queue<Vector2Int>();
            queue.Enqueue(start);
            seen[start.x, start.y] = true;
            Vector2Int[] steps = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };
            while (queue.Count > 0)
            {
                Vector2Int t = queue.Dequeue();
                foreach (Vector2Int step in steps)
                {
                    Vector2Int n = t + step;
                    if (layout.InBounds(n.x, n.y) && !seen[n.x, n.y] && IsWalkable(layout[n.x, n.y]))
                    {
                        seen[n.x, n.y] = true;
                        queue.Enqueue(n);
                    }
                }
            }

            for (int x = 0; x < size.x; x++)
            {
                for (int z = 0; z < size.y; z++)
                {
                    if (IsWalkable(layout[x, z]) && !seen[x, z])
                    {
                        errors.Add($"{layout.Name}: tile at {Describe(layout, new Vector2Int(x, z))} can't be reached from the doors.");
                        return;
                    }
                }
            }
        }

        /// <summary>Position as the designer sees it in the text: row 1 = top line, column 1 = first character.</summary>
        private static string Describe(RoomLayout layout, Vector2Int tile) =>
            $"row {layout.Size.y - tile.y}, column {tile.x + 1}";
    }
}
