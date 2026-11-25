using System;
using System.Collections.Generic;
using Game.Core;
using UnityEngine;

namespace Game.Combat
{
    /// <summary>
    /// A short-range swing in an arc towards the aim. Hits every damageable of the other team
    /// inside the arc once per swing, with knockback. Has its own cooldown.
    /// </summary>
    public sealed class MeleeAttack : MonoBehaviour
    {
        private static readonly Collider[] OverlapBuffer = new Collider[32];

        [SerializeField] private MeleeAttackConfig _config;

        [Tooltip("Component implementing IAimProvider (e.g. PlayerAim).")]
        [SerializeField] private MonoBehaviour _aimSource;

        [Tooltip("The attacker's own Health: gives its team and stops swings after death.")]
        [SerializeField] private Health _owner;

        private readonly HashSet<IDamageable> _hitThisSwing = new HashSet<IDamageable>();
        private IAimProvider _aim;
        private float _readyTime;

        /// <summary>Every swing, hit or miss. Carries the swing direction (for visuals).</summary>
        public event Action<Vector3> Swung;

        /// <summary>A swing that damaged at least one target. Carries how many (for hit stop / shake).</summary>
        public event Action<int> Hit;

        public MeleeAttackConfig Config => _config;
        public bool IsReady => Time.time >= _readyTime;

        private void Awake()
        {
            _aim = _aimSource as IAimProvider;
            if (_aim == null)
                throw new InvalidOperationException($"{name}: MeleeAttack needs an aim source that implements IAimProvider.");
        }

        /// <summary>Swings if the cooldown allows. Returns false if it didn't swing.</summary>
        public bool TryAttack()
        {
            if (!isActiveAndEnabled || !IsReady || (_owner != null && _owner.IsDead))
                return false;

            _readyTime = Time.time + _config.Cooldown;
            Vector3 direction = _aim.AimDirection;
            Vector3 centre = transform.position + Vector3.up * _config.Height;
            int hits = 0;

            int count = Physics.OverlapSphereNonAlloc(centre, _config.Range, OverlapBuffer, Physics.AllLayers, QueryTriggerInteraction.Ignore);
            for (int i = 0; i < count; i++)
            {
                Collider other = OverlapBuffer[i];
                IDamageable target = other.GetComponentInParent<IDamageable>();
                if (target == null || (_owner != null && target.Team == _owner.Team) || !_hitThisSwing.Add(target))
                    continue;

                Vector3 toTarget = other.ClosestPoint(centre) - centre;
                if (!PlanarMath.IsWithinArc(direction, toTarget, _config.ArcDegrees))
                    continue;

                // Push away from the attacker; fall back to the swing direction when overlapping.
                Vector3 push = PlanarMath.Direction(transform.position, other.transform.position, direction);
                if (target.TryTakeDamage(new DamageInfo(_config.Damage, push, _config.KnockbackForce, gameObject)))
                    hits++;
            }

            _hitThisSwing.Clear();
            Swung?.Invoke(direction);
            if (hits > 0)
                Hit?.Invoke(hits);
            return true;
        }
    }
}
