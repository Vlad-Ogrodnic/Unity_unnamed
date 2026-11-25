using System;

namespace Game.Core
{
    /// <summary>Hit points and i-frames as plain logic. Time is passed in, so it can be tested without a scene.</summary>
    public sealed class HealthState
    {
        private float _invulnerableUntil = float.NegativeInfinity;

        public int Max { get; }
        public int Current { get; private set; }
        public float InvulnerabilityDuration { get; }
        public bool IsDead => Current <= 0;

        public HealthState(int max, float invulnerabilityDuration)
        {
            if (max <= 0)
                throw new ArgumentOutOfRangeException(nameof(max), "Max health must be positive.");

            Max = max;
            Current = max;
            InvulnerabilityDuration = Math.Max(0f, invulnerabilityDuration);
        }

        public bool IsInvulnerable(float now) => now < _invulnerableUntil;

        /// <summary>
        /// Applies damage unless dead, invulnerable or the amount is not positive.
        /// <paramref name="died"/> is true only on the hit that brings health to 0.
        /// </summary>
        public bool TryDamage(int amount, float now, out bool died)
        {
            died = false;
            if (amount <= 0 || IsDead || IsInvulnerable(now))
                return false;

            Current = Math.Max(0, Current - amount);
            died = IsDead;
            if (!died && InvulnerabilityDuration > 0f)
                _invulnerableUntil = now + InvulnerabilityDuration;
            return true;
        }

        /// <summary>Heals up to max. Returns how much was actually restored (0 if dead).</summary>
        public int Heal(int amount)
        {
            if (amount <= 0 || IsDead)
                return 0;

            int healed = Math.Min(amount, Max - Current);
            Current += healed;
            return healed;
        }
    }
}
