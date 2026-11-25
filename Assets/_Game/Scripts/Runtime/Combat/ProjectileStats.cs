namespace Game.Combat
{
    /// <summary>What a projectile does once fired. Built by the attack from its config (later: config + stat modifiers).</summary>
    public readonly struct ProjectileStats
    {
        public readonly int Damage;
        public readonly float Speed;
        public readonly float Range;
        public readonly float Radius;
        public readonly float KnockbackForce;

        public ProjectileStats(int damage, float speed, float range, float radius, float knockbackForce)
        {
            Damage = damage;
            Speed = speed;
            Range = range;
            Radius = radius;
            KnockbackForce = knockbackForce;
        }
    }
}
