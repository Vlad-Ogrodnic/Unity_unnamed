using UnityEngine;

namespace Game.Enemies
{
    /// <summary>Charger: waits, telegraphs, then charges in a straight line until a wall (FEATURE_PLAN M2-07).</summary>
    [CreateAssetMenu(menuName = "Game/Enemy Config (Charger)", fileName = "EnemyConfig_Charger")]
    public sealed class ChargerConfig : EnemyConfig
    {
        [Tooltip("Seconds standing still before telegraphing the next charge.")]
        [SerializeField, Min(0f)] private float _waitDuration = 0.7f;

        [Tooltip("Seconds of blinking before the charge (FEATURE_PLAN: ~0.5).")]
        [SerializeField, Min(0f)] private float _telegraphDuration = 0.5f;

        [Tooltip("Charge speed in m/s.")]
        [SerializeField, Min(0f)] private float _chargeSpeed = 12f;

        [Tooltip("Safety limit: a charge that hits nothing stops after this many seconds.")]
        [SerializeField, Min(0.1f)] private float _maxChargeDuration = 2f;

        [Tooltip("Seconds standing still after a charge ends.")]
        [SerializeField, Min(0f)] private float _recoverDuration = 0.8f;

        public float WaitDuration => _waitDuration;
        public float TelegraphDuration => _telegraphDuration;
        public float ChargeSpeed => _chargeSpeed;
        public float MaxChargeDuration => _maxChargeDuration;
        public float RecoverDuration => _recoverDuration;
    }
}
