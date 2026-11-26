namespace Game.Core
{
    /// <summary>Anything that can be hit. Attacks talk to this, never to concrete enemy or player types.</summary>
    public interface IDamageable
    {
        Team Team { get; }

        /// <summary>Applies the hit. Returns false if it was ignored (invulnerable, i-frames or already dead).</summary>
        bool TryTakeDamage(in DamageInfo damage);
    }
}
