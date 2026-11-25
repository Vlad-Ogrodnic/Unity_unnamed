using System;
using UnityEngine;

namespace Game.Dungeon
{
    /// <summary>How floors are generated (GDD ch. 2, Structura unui etaj).</summary>
    [CreateAssetMenu(menuName = "Game/Floor Generation Config", fileName = "FloorGenerationConfig")]
    public sealed class FloorGenerationConfig : ScriptableObject
    {
        [Serializable]
        public struct ShapeWeight
        {
            [Tooltip("Room size in cells: 1x1, 2x1, 1x2 or 2x2.")]
            public Vector2Int Size;

            [Min(0f)] public float Weight;
        }

        [Tooltip("Combat rooms per floor (the start room comes on top).")]
        [SerializeField, Min(1)] private int _combatRoomCount = 8;

        [Tooltip("How often each room size is picked. Sizes without templates are skipped.")]
        [SerializeField] private ShapeWeight[] _shapes =
        {
            new ShapeWeight { Size = new Vector2Int(1, 1), Weight = 6f },
            new ShapeWeight { Size = new Vector2Int(2, 1), Weight = 2f },
            new ShapeWeight { Size = new Vector2Int(1, 2), Weight = 1f },
            new ShapeWeight { Size = new Vector2Int(2, 2), Weight = 1f },
        };

        [Tooltip("0 = a new random floor every time. Any other value always gives the same floor (for debugging).")]
        [SerializeField] private int _fixedSeed;

        public int CombatRoomCount => _combatRoomCount;
        public ShapeWeight[] Shapes => _shapes;
        public int FixedSeed => _fixedSeed;
    }
}
