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

        public float MoveSpeed => _moveSpeed;
        public float KnockbackDuration => _knockbackDuration;
    }
}
