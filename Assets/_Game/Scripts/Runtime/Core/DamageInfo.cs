using UnityEngine;

namespace Game.Core
{
    /// <summary>One hit: how much damage, where it came from and how hard it pushes.</summary>
    public readonly struct DamageInfo
    {
        public readonly int Amount;

        /// <summary>Knockback direction, normalized on the XZ plane (zero if none).</summary>
        public readonly Vector3 Direction;

        public readonly float KnockbackForce;

        /// <summary>The object that caused the hit (attacker, projectile, bomb). May be null.</summary>
        public readonly GameObject Source;

        public DamageInfo(int amount, Vector3 direction, float knockbackForce, GameObject source)
        {
            Amount = amount;
            Direction = PlanarMath.FlattenAndNormalize(direction);
            KnockbackForce = knockbackForce;
            Source = source;
        }
    }
}
