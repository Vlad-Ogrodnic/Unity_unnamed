using Game.Core;
using UnityEngine;

namespace Game.Enemies
{
    /// <summary>Values for one enemy type (one asset per type, e.g. EnemyConfig_Chaser).</summary>
    [CreateAssetMenu(menuName = "Game/Enemy Config", fileName = "EnemyConfig")]
    public sealed class EnemyConfig : HealthConfig
    {
        [Tooltip("Movement speed in m/s.")]
        [SerializeField, Min(0f)] private float _moveSpeed = 3f;

        [Tooltip("Damage dealt to the player on contact (GDD: 1, stronger enemies 2).")]
        [SerializeField, Min(0)] private int _contactDamage = 1;

        [Tooltip("Knockback speed given to the player on contact, in m/s.")]
        [SerializeField, Min(0f)] private float _contactKnockbackForce = 6f;

        [Tooltip("Seconds a knockback takes to fade out for this enemy.")]
        [SerializeField, Min(0f)] private float _knockbackDuration = 0.15f;

        public float MoveSpeed => _moveSpeed;
        public int ContactDamage => _contactDamage;
        public float ContactKnockbackForce => _contactKnockbackForce;
        public float KnockbackDuration => _knockbackDuration;
    }
}
