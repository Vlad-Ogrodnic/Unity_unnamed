using System.Collections.Generic;
using Game.Rooms;
using UnityEngine;

namespace Game.Dungeon
{
    public enum RoomType
    {
        Start,
        Combat
    }

    /// <summary>A room placed on the floor grid. Origin is its south-west cell.</summary>
    public sealed class PlacedRoom
    {
        public int Index { get; }
        public Vector2Int Origin { get; }
        public Vector2Int Size { get; }
        public RoomType Type { get; }

        public PlacedRoom(int index, Vector2Int origin, Vector2Int size, RoomType type)
        {
            Index = index;
            Origin = origin;
            Size = size;
            Type = type;
        }

        public IEnumerable<Vector2Int> CoveredCells()
        {
            for (int x = 0; x < Size.x; x++)
                for (int y = 0; y < Size.y; y++)
                    yield return Origin + new Vector2Int(x, y);
        }
    }

    /// <summary>A door pair between two neighbouring rooms, described from room A's side.</summary>
    public readonly struct RoomConnection
    {
        public readonly int RoomA;
        public readonly int RoomB;

        /// <summary>The grid cell of room A that has the door.</summary>
        public readonly Vector2Int CellA;
        public readonly DoorSide SideA;

        public RoomConnection(int roomA, int roomB, Vector2Int cellA, DoorSide sideA)
        {
            RoomA = roomA;
            RoomB = roomB;
            CellA = cellA;
            SideA = sideA;
        }

        public Vector2Int CellB => CellA + DoorSlot.Offset(SideA);
        public DoorSide SideB => DoorSlot.Opposite(SideA);
    }

    /// <summary>Result of floor generation: which rooms sit where and which doors connect them. Plain data.</summary>
    public sealed class FloorLayout
    {
        private readonly Dictionary<Vector2Int, int> _cells = new Dictionary<Vector2Int, int>();
        private readonly List<int>[] _neighbours;

        public IReadOnlyList<PlacedRoom> Rooms { get; }
        public IReadOnlyList<RoomConnection> Connections { get; }
        public int Seed { get; }

        public FloorLayout(int seed, IReadOnlyList<PlacedRoom> rooms, IReadOnlyList<RoomConnection> connections)
        {
            Seed = seed;
            Rooms = rooms;
            Connections = connections;
            _neighbours = new List<int>[rooms.Count];
            for (int i = 0; i < rooms.Count; i++)
            {
                _neighbours[i] = new List<int>();
                foreach (Vector2Int cell in rooms[i].CoveredCells())
                    _cells[cell] = i;
            }
            foreach (RoomConnection c in connections)
            {
                if (!_neighbours[c.RoomA].Contains(c.RoomB)) _neighbours[c.RoomA].Add(c.RoomB);
                if (!_neighbours[c.RoomB].Contains(c.RoomA)) _neighbours[c.RoomB].Add(c.RoomA);
            }
        }

        /// <summary>Index of the room covering a cell, or -1.</summary>
        public int RoomAt(Vector2Int cell) => _cells.TryGetValue(cell, out int index) ? index : -1;

        /// <summary>Rooms reachable through one door from room <paramref name="index"/>.</summary>
        public IReadOnlyList<int> Neighbours(int index) => _neighbours[index];
    }
}
