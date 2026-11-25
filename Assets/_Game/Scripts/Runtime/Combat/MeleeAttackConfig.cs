using UnityEngine;

namespace Game.Combat
{
    /// <summary>Values for a melee attack. Starting values from GDD ch. 4, Atacul melee.</summary>
    [CreateAssetMenu(menuName = "Game/Melee Attack Config", fileName = "MeleeAttackConfig")]
    public sealed class MeleeAttackConfig : ScriptableObject
    {
        [Tooltip("GDD: about 3x the damage of one projectile.")]
        [SerializeField, Min(0)] private int _damage = 3;

        [Tooltip("Width of the hit arc in degrees, centred on the aim (GDD: ~120; 90 after the first playtest).")]
        [SerializeField, Range(10f, 360f)] private float _arcDegrees = 90f;

        [Tooltip("Reach from the attacker's centre, in m (GDD: short).")]
        [SerializeField, Min(0.1f)] private float _range = 1.6f;

        [Tooltip("Knockback speed given to each target, in m/s.")]
        [SerializeField, Min(0f)] private float _knockbackForce = 9f;

        [Tooltip("Seconds between swings (GDD: ~0.8). Separate from the ranged attack.")]
        [SerializeField, Min(0f)] private float _cooldown = 0.8f;

        [Tooltip("Height above the attacker's feet where the hit sphere is centred.")]
        [SerializeField, Min(0f)] private float _height = 0.5f;

        public int Damage => _damage;
        public float ArcDegrees => _arcDegrees;
        public float Range => _range;
        public float KnockbackForce => _knockbackForce;
        public float Cooldown => _cooldown;
        public float Height => _height;
    }
}
