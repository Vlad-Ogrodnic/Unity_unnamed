using UnityEngine;

namespace Game.Core
{
    /// <summary>
    /// Health values for anything with <see cref="Health"/>. Character configs
    /// (<c>PlayerConfig</c>, <c>EnemyConfig</c>) extend it so one asset holds all of a character's values.
    /// </summary>
    [CreateAssetMenu(menuName = "Game/Health Config", fileName = "HealthConfig")]
    public class HealthConfig : ScriptableObject
    {
        [SerializeField, Min(1)] private int _maxHealth = 3;

        [Tooltip("Seconds of invulnerability after taking damage (i-frames). 0 = none.")]
        [SerializeField, Min(0f)] private float _invulnerabilityDuration;

        public int MaxHealth => _maxHealth;
        public float InvulnerabilityDuration => _invulnerabilityDuration;
    }
}
