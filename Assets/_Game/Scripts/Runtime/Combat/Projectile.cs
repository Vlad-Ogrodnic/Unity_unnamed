using System;
using Game.Core;
using UnityEngine;

namespace Game.Combat
{
    /// <summary>
    /// A straight-flying projectile. Each frame it sphere-casts along its path, so it can't
    /// skip through thin colliders at high speed. What it can hit comes from the collision
    /// matrix for its own layer (PlayerProjectile / EnemyProjectile), not from code.
    /// </summary>
    public sealed class Projectile : MonoBehaviour
    {
        private static readonly Collider[] OverlapBuffer = new Collider[4];

        private ProjectileStats _stats;
        private Vector3 _direction;
        private float _remainingRange;
        private GameObject _source;
        private Action<Projectile> _release;
        private int _hitMask;
        private bool _flying;
        private bool _checkSpawnOverlap;

        private void Awake()
        {
            _hitMask = PhysicsLayers.CollisionMaskFor(gameObject.layer);
        }

        /// <param name="direction">Flight direction; flattened to XZ.</param>
        /// <param name="stats">Damage, speed, range, radius, knockback.</param>
        /// <param name="source">Who fired it; passed on in <see cref="DamageInfo.Source"/>.</param>
        /// <param name="release">Returns the projectile to its pool when it hits or runs out of range.</param>
        public void Launch(Vector3 direction, in ProjectileStats stats, GameObject source, Action<Projectile> release)
        {
            _direction = PlanarMath.FlattenAndNormalize(direction);
            _stats = stats;
            _remainingRange = stats.Range;
            _source = source;
            _release = release;
            _flying = _direction != Vector3.zero;
            _checkSpawnOverlap = true;
            if (!_flying)
                Despawn();
        }

        private void Update()
        {
            if (!_flying)
                return;

            Vector3 position = transform.position;

            // A sphere cast misses colliders it starts inside of (e.g. firing point-blank into a wall).
            if (_checkSpawnOverlap)
            {
                _checkSpawnOverlap = false;
                int count = Physics.OverlapSphereNonAlloc(position, _stats.Radius, OverlapBuffer, _hitMask, QueryTriggerInteraction.Ignore);
                if (count > 0)
                {
                    Hit(OverlapBuffer[0]);
                    return;
                }
            }

            float step = Mathf.Min(_stats.Speed * Time.deltaTime, _remainingRange);
            if (Physics.SphereCast(position, _stats.Radius, _direction, out RaycastHit hit, step, _hitMask, QueryTriggerInteraction.Ignore))
            {
                transform.position = position + _direction * hit.distance;
                Hit(hit.collider);
                return;
            }

            transform.position = position + _direction * step;
            _remainingRange -= step;
            if (_remainingRange <= 0f)
                Despawn();
        }

        private void Hit(Collider other)
        {
            IDamageable target = other.GetComponentInParent<IDamageable>();
            target?.TryTakeDamage(new DamageInfo(_stats.Damage, _direction, _stats.KnockbackForce, _source));
            Despawn();
        }

        private void Despawn()
        {
            _flying = false;
            Action<Projectile> release = _release;
            _release = null;
            if (release != null)
                release(this);
            else
                gameObject.SetActive(false);
        }
    }
}
