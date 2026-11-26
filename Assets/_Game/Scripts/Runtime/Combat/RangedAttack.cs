using System;
using Game.Core;
using UnityEngine;

namespace Game.Combat
{
    /// <summary>
    /// Fires projectiles towards the aim while <see cref="TriggerHeld"/> is true.
    /// Doesn't know who pulls the trigger: the player's input or an enemy's AI.
    /// </summary>
    public sealed class RangedAttack : MonoBehaviour
    {
        // Projectiles created up front, so the first shots of a fight don't instantiate.
        private const int PoolPrewarm = 8;

        [SerializeField] private RangedAttackConfig _config;

        [Tooltip("Component implementing IAimProvider (e.g. PlayerAim).")]
        [SerializeField] private MonoBehaviour _aimSource;

        private IAimProvider _aim;
        private PrefabPool<Projectile> _pool;
        private Action<Projectile> _release;
        private Transform _projectileContainer;
        private float _nextShotTime;

        public bool TriggerHeld { get; set; }

        private void Awake()
        {
            _aim = _aimSource as IAimProvider;
            if (_aim == null)
                throw new InvalidOperationException($"{name}: RangedAttack needs an aim source that implements IAimProvider.");

            // Projectiles live outside the shooter so they don't move with it.
            _projectileContainer = new GameObject($"{name} Projectiles").transform;
            _pool = new PrefabPool<Projectile>(_config.ProjectilePrefab, _projectileContainer);
            _pool.Prewarm(PoolPrewarm);
            _release = _pool.Release;
        }

        private void OnDestroy()
        {
            _pool?.Dispose();
            if (_projectileContainer != null)
                Destroy(_projectileContainer.gameObject);
        }

        private void Update()
        {
            if (!TriggerHeld || Time.time < _nextShotTime)
                return;

            Fire();
            _nextShotTime = Time.time + 1f / _config.FireRate;
        }

        private void Fire()
        {
            Vector3 direction = _aim.AimDirection;
            Vector3 origin = transform.position + Vector3.up * _config.SpawnHeight + direction * _config.SpawnOffset;
            Projectile projectile = _pool.Get(origin, Quaternion.LookRotation(direction, Vector3.up));
            projectile.Launch(direction, _config.Stats, gameObject, _release);
        }
    }
}
