using UnityEngine;

namespace Game.Combat
{
    /// <summary>Values for a ranged attack. Starting values from GDD ch. 4, Atacul ranged.</summary>
    [CreateAssetMenu(menuName = "Game/Ranged Attack Config", fileName = "RangedAttackConfig")]
    public sealed class RangedAttackConfig : ScriptableObject
    {
        [SerializeField] private Projectile _projectilePrefab;

        [Tooltip("Shots per second while the trigger is held.")]
        [SerializeField, Min(0.01f)] private float _fireRate = 3f;

        [SerializeField, Min(0)] private int _damage = 1;

        [Tooltip("Projectile speed in m/s.")]
        [SerializeField, Min(0.1f)] private float _projectileSpeed = 11f;

        [Tooltip("Distance in m before the projectile disappears (about half a room's width).")]
        [SerializeField, Min(0.1f)] private float _range = 10f;

        [Tooltip("Radius of the projectile's hit sphere, in m.")]
        [SerializeField, Min(0.01f)] private float _projectileRadius = 0.15f;

        [Tooltip("Knockback speed applied to the target, in m/s. 0 = none.")]
        [SerializeField, Min(0f)] private float _knockbackForce;

        [Tooltip("Height above the shooter's feet where projectiles spawn and fly.")]
        [SerializeField, Min(0f)] private float _spawnHeight = 0.5f;

        [Tooltip("Distance from the shooter's centre, along the aim, where projectiles spawn.")]
        [SerializeField, Min(0f)] private float _spawnOffset = 0.5f;

        public Projectile ProjectilePrefab => _projectilePrefab;
        public float FireRate => _fireRate;
        public float SpawnHeight => _spawnHeight;
        public float SpawnOffset => _spawnOffset;

        public ProjectileStats Stats => new ProjectileStats(_damage, _projectileSpeed, _range, _projectileRadius, _knockbackForce);
    }
}
