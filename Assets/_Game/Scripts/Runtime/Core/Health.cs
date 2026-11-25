using System;
using UnityEngine;

namespace Game.Core
{
    /// <summary>Hit points for the player, enemies and anything else that can be hit.</summary>
    [DisallowMultipleComponent]
    public sealed class Health : MonoBehaviour, IDamageable
    {
        [SerializeField] private HealthConfig _config;
        [SerializeField] private Team _team;

        private HealthState _state;

        public event Action<DamageInfo> Damaged;
        public event Action<int> Healed;
        public event Action Died;

        public Team Team => _team;
        public int Current => State.Current;
        public int Max => State.Max;
        public bool IsDead => State.IsDead;
        public bool IsInvulnerable => State.IsInvulnerable(Time.time);

        private HealthState State =>
            _state ?? throw new InvalidOperationException($"{name}: Health has no config. Assign one or call Configure().");

        private void Awake()
        {
            if (_config != null)
                Configure(_config, _team);
        }

        /// <summary>Sets the config and team and restores full health. For objects built from code (spawners, tests).</summary>
        public void Configure(HealthConfig config, Team team)
        {
            _config = config != null ? config : throw new ArgumentNullException(nameof(config));
            _team = team;
            _state = new HealthState(config.MaxHealth, config.InvulnerabilityDuration);
        }

        public bool TryTakeDamage(in DamageInfo damage)
        {
            if (!State.TryDamage(damage.Amount, Time.time, out bool died))
                return false;

            Damaged?.Invoke(damage);
            if (died)
                Died?.Invoke();
            return true;
        }

        public int Heal(int amount)
        {
            int healed = State.Heal(amount);
            if (healed > 0)
                Healed?.Invoke(healed);
            return healed;
        }
    }
}
