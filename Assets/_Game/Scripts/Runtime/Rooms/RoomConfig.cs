using UnityEngine;

namespace Game.Rooms
{
    /// <summary>What a cleared room may drop (GDD ch. 2, Bucla unui room).</summary>
    [CreateAssetMenu(menuName = "Game/Room Config", fileName = "RoomConfig")]
    public sealed class RoomConfig : ScriptableObject
    {
        [Tooltip("Chance (0-1) that a cleared combat room drops a potion. Coins come with M4-02.")]
        [SerializeField, Range(0f, 1f)] private float _potionDropChance = 0.35f;

        [SerializeField] private GameObject _potionPickupPrefab;

        public float PotionDropChance => _potionDropChance;
        public GameObject PotionPickupPrefab => _potionPickupPrefab;
    }
}
