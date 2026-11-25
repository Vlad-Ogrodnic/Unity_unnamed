using System;
using System.Collections.Generic;
using Game.Rooms;
using UnityEngine;

namespace Game.Dungeon
{
    /// <summary>
    /// Grows a floor from a 1×1 start room: repeatedly picks a placed room and one of its door
    /// slots, and puts a new room of a random size behind it if the cells are free. A new room
    /// may touch at most <see cref="MaxTouchedRooms"/> existing rooms, which keeps floors branching
    /// instead of clumping. Every pair of neighbouring rooms gets connected by a door.
    /// Same seed, same floor.
    /// </summary>
    public static class FloorLayoutGenerator
    {
        public const int MaxTouchedRooms = 2;
        private const int AttemptsPerRoom = 200;

        public static FloorLayout Generate(int seed, int combatRoomCount, IReadOnlyList<FloorGenerationConfig.ShapeWeight> shapes)
        {
            if (combatRoomCount < 0)
                throw new ArgumentOutOfRangeException(nameof(combatRoomCount));

            var random = new System.Random(seed);
            var rooms = new List<PlacedRoom> { new PlacedRoom(0, Vector2Int.zero, Vector2Int.one, RoomType.Start) };
            var occupied = new Dictionary<Vector2Int, int> { { Vector2Int.zero, 0 } };
            float totalWeight = 0f;
            foreach (FloorGenerationConfig.ShapeWeight s in shapes)
                totalWeight += Mathf.Max(0f, s.Weight);
            if (totalWeight <= 0f)
                throw new ArgumentException("At least one room shape needs a positive weight.", nameof(shapes));

            int attempts = 0;
            while (rooms.Count < combatRoomCount + 1)
            {
                if (attempts++ > AttemptsPerRoom * (combatRoomCount + 1))
                    throw new InvalidOperationException($"Couldn't place {combatRoomCount} rooms (seed {seed}).");

                PlacedRoom from = rooms[random.Next(rooms.Count)];
                DoorSlot slot = RandomSlot(from.Size, random);
                Vector2Int target = from.Origin + slot.Cell + DoorSlot.Offset(slot.Side);
                if (occupied.ContainsKey(target))
                    continue;

                Vector2Int size = PickShape(shapes, totalWeight, random);
                if (TryPlace(target, size, occupied, random, out Vector2Int origin))
                {
                    var room = new PlacedRoom(rooms.Count, origin, size, RoomType.Combat);
                    rooms.Add(room);
                    foreach (Vector2Int cell in room.CoveredCells())
                        occupied[cell] = room.Index;
                }
            }

            return new FloorLayout(seed, rooms, Connect(rooms, occupied));
        }

        private static DoorSlot RandomSlot(Vector2Int size, System.Random random)
        {
            var slots = new List<DoorSlot>(DoorSlot.All(size));
            return slots[random.Next(slots.Count)];
        }

        private static Vector2Int PickShape(IReadOnlyList<FloorGenerationConfig.ShapeWeight> shapes, float totalWeight, System.Random random)
        {
            double roll = random.NextDouble() * totalWeight;
            foreach (FloorGenerationConfig.ShapeWeight s in shapes)
            {
                roll -= Mathf.Max(0f, s.Weight);
                if (roll < 0)
                    return s.Size;
            }
            return shapes[shapes.Count - 1].Size;
        }

        /// <summary>Tries every position of the shape that covers <paramref name="target"/>, in random order.</summary>
        private static bool TryPlace(Vector2Int target, Vector2Int size, Dictionary<Vector2Int, int> occupied, System.Random random, out Vector2Int origin)
        {
            var origins = new List<Vector2Int>();
            for (int dx = 0; dx < size.x; dx++)
                for (int dy = 0; dy < size.y; dy++)
                    origins.Add(target - new Vector2Int(dx, dy));
            for (int i = origins.Count - 1; i > 0; i--)
            {
                int j = random.Next(i + 1);
                (origins[i], origins[j]) = (origins[j], origins[i]);
            }

            var touched = new HashSet<int>();
            foreach (Vector2Int candidate in origins)
            {
                touched.Clear();
                bool free = true;
                for (int x = 0; x < size.x && free; x++)
                {
                    for (int y = 0; y < size.y && free; y++)
                    {
                        Vector2Int cell = candidate + new Vector2Int(x, y);
                        if (occupied.ContainsKey(cell))
                        {
                            free = false;
                            break;
                        }
                        foreach (DoorSide side in AllSides)
                        {
                            if (occupied.TryGetValue(cell + DoorSlot.Offset(side), out int neighbour))
                                touched.Add(neighbour);
                        }
                    }
                }

                if (free && touched.Count <= MaxTouchedRooms)
                {
                    origin = candidate;
                    return true;
                }
            }

            origin = default;
            return false;
        }

        private static readonly DoorSide[] AllSides = { DoorSide.North, DoorSide.East, DoorSide.South, DoorSide.West };

        private static List<RoomConnection> Connect(List<PlacedRoom> rooms, Dictionary<Vector2Int, int> occupied)
        {
            var connections = new List<RoomConnection>();
            foreach (PlacedRoom room in rooms)
            {
                foreach (DoorSlot slot in DoorSlot.All(room.Size))
                {
                    Vector2Int cell = room.Origin + slot.Cell;
                    if (occupied.TryGetValue(cell + DoorSlot.Offset(slot.Side), out int other) && other > room.Index)
                        connections.Add(new RoomConnection(room.Index, other, cell, slot.Side));
                }
            }
            return connections;
        }
    }
}
