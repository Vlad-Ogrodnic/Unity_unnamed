using Game.Combat;
using UnityEngine;

namespace Game.Feedback
{
    /// <summary>Plays hit stop and screen shake when the player's melee lands (bomb explosions plug in here later).</summary>
    public sealed class ImpactFeedback : MonoBehaviour
    {
        [SerializeField] private FeedbackConfig _config;
        [SerializeField] private MeleeAttack _melee;
        [SerializeField] private HitStop _hitStop;
        [SerializeField] private ScreenShake _screenShake;

        private void OnEnable() => _melee.Hit += OnMeleeHit;

        private void OnDisable() => _melee.Hit -= OnMeleeHit;

        private void OnMeleeHit(int _)
        {
            _hitStop.Play(_config.HitStopDuration);
            _screenShake.Shake(_config.ScreenShakeStrength, _config.ScreenShakeDuration);
        }
    }
}
