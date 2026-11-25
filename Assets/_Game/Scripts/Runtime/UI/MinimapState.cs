using System.Collections.Generic;

namespace Game.UI
{
    /// <summary>
    /// What the minimap knows (GDD ch. 2): rooms the player visited, the current room, and
    /// rooms seen through a door of a visited room. Everything else stays hidden.
    /// </summary>
    public sealed class MinimapState
    {
        public enum RoomVisibility
        {
            Hidden,
            Discovered,
            Visited
        }

        private readonly RoomVisibility[] _rooms;

        public int Current { get; private set; } = -1;

        public MinimapState(int roomCount)
        {
            _rooms = new RoomVisibility[roomCount];
        }

        public RoomVisibility this[int room] => _rooms[room];

        public void Enter(int room, IReadOnlyList<int> neighbours)
        {
            Current = room;
            _rooms[room] = RoomVisibility.Visited;
            foreach (int neighbour in neighbours)
            {
                if (_rooms[neighbour] == RoomVisibility.Hidden)
                    _rooms[neighbour] = RoomVisibility.Discovered;
            }
        }
    }
}
