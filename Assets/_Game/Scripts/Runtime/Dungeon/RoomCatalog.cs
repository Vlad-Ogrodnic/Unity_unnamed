using System.Collections.Generic;
using Game.Rooms;
using UnityEngine;

namespace Game.Dungeon
{
    /// <summary>The room templates a floor is built from. Filled by Game → Rooms → Rebuild Room Prefabs.</summary>
    [CreateAssetMenu(menuName = "Game/Room Catalog", fileName = "RoomCatalog")]
    public sealed class RoomCatalog : ScriptableObject
    {
        [SerializeField] private Room[] _startRooms = System.Array.Empty<Room>();
        [SerializeField] private Room[] _combatRooms = System.Array.Empty<Room>();

        public IReadOnlyList<Room> StartRooms => _startRooms;
        public IReadOnlyList<Room> CombatRooms => _combatRooms;

        public bool HasCombatRoom(Vector2Int size)
        {
            foreach (Room room in _combatRooms)
            {
                if (room.Cells == size)
                    return true;
            }
            return false;
        }

        /// <summary>A random template of the given type and size (null if none).</summary>
        public Room Pick(RoomType type, Vector2Int size, System.Random random)
        {
            Room[] source = type == RoomType.Start ? _startRooms : _combatRooms;
            var matches = new List<Room>();
            foreach (Room room in source)
            {
                if (room.Cells == size)
                    matches.Add(room);
            }
            return matches.Count > 0 ? matches[random.Next(matches.Count)] : null;
        }
    }
}
