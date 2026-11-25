using UnityEngine;

namespace Game.Enemies
{
    /// <summary>Shooter: keeps its distance and fires slow projectiles at the player (FEATURE_PLAN M2-06).</summary>
    [CreateAssetMenu(menuName = "Game/Enemy Config (Shooter)", fileName = "EnemyConfig_Shooter")]
    public sealed class ShooterConfig : EnemyConfig
    {
        [Tooltip("Closer than this, the shooter backs away (m).")]
        [SerializeField, Min(0f)] private float _minDistance = 4f;

        [Tooltip("Farther than this, the shooter walks closer (m).")]
        [SerializeField, Min(0f)] private float _maxDistance = 7f;

        [Tooltip("Fires only while the player is within this distance (m).")]
        [SerializeField, Min(0f)] private float _fireDistance = 9f;

        public float MinDistance => _minDistance;
        public float MaxDistance => Mathf.Max(_minDistance, _maxDistance);
        public float FireDistance => _fireDistance;
    }
}
