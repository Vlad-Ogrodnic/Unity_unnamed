using Game.Core;
using UnityEngine;

namespace Game.Player
{
    /// <summary>Base values for the player. Starting values from GDD ch. 3 and 4.</summary>
    [CreateAssetMenu(menuName = "Game/Player Config", fileName = "PlayerConfig")]
    public sealed class PlayerConfig : HealthConfig
    {
        [Tooltip("Movement speed in m/s.")]
        [SerializeField, Min(0f)] private float _moveSpeed = 5.5f;

        [Tooltip("Seconds a knockback takes to fade out; movement input is ignored meanwhile.")]
        [SerializeField, Min(0f)] private float _knockbackDuration = 0.15f;

        [Tooltip("Most potions the player can carry (GDD: 3).")]
        [SerializeField, Min(0)] private int _maxPotions = 3;

        [Tooltip("HP restored by one potion (GDD: 2).")]
        [SerializeField, Min(1)] private int _potionHealAmount = 2;

        public float MoveSpeed => _moveSpeed;
        public float KnockbackDuration => _knockbackDuration;
        public int MaxPotions => _maxPotions;
        public int PotionHealAmount => _potionHealAmount;
    }
}
