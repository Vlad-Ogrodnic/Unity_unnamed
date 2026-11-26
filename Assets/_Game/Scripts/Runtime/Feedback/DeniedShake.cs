using Game.Player;
using UnityEngine;

namespace Game.Feedback
{
    /// <summary>Lives on the player's <c>Visual</c>: a short side-to-side wiggle when a potion can't be drunk.</summary>
    public sealed class DeniedShake : MonoBehaviour
    {
        // Wiggles per second.
        private const float Frequency = 30f;

        [SerializeField] private PlayerPotionUse _potionUse;
        [SerializeField] private FeedbackConfig _config;

        private Vector3 _restPosition;
        private float _endTime;
        private bool _shaking;

        private void Awake()
        {
            _restPosition = transform.localPosition;
        }

        private void OnEnable() => _potionUse.UseFailed += OnUseFailed;

        private void OnDisable()
        {
            _potionUse.UseFailed -= OnUseFailed;
            _shaking = false;
            transform.localPosition = _restPosition;
        }

        private void Update()
        {
            if (!_shaking)
                return;

            float remaining = _endTime - Time.unscaledTime;
            if (remaining <= 0f)
            {
                _shaking = false;
                transform.localPosition = _restPosition;
                return;
            }

            float fade = remaining / _config.DeniedShakeDuration;
            float offset = Mathf.Sin(Time.unscaledTime * Frequency * 2f * Mathf.PI) * _config.DeniedShakeAmplitude * fade;
            transform.localPosition = _restPosition + Vector3.right * offset;
        }

        private void OnUseFailed()
        {
            _shaking = true;
            _endTime = Time.unscaledTime + _config.DeniedShakeDuration;
        }
    }
}
