using Game.Core;
using UnityEngine;

namespace Game.Feedback
{
    /// <summary>
    /// Lives on <c>Visual</c>: turns its renderers white for a moment when the root's
    /// <see cref="Health"/> takes damage.
    /// </summary>
    public sealed class HitFlash : MonoBehaviour
    {
        [SerializeField] private Health _health;
        [SerializeField] private FeedbackConfig _config;
        [Tooltip("Renderers to affect. Empty = every renderer under this object.")]
        [SerializeField] private Renderer[] _renderers = System.Array.Empty<Renderer>();

        private RendererFlash _flash;
        private float _flashEndTime;

        private void Awake()
        {
            _flash = new RendererFlash(TargetRenderers(), _config.HitFlashMaterial);
        }

        private void OnEnable() => _health.Damaged += OnDamaged;

        private void OnDisable()
        {
            _health.Damaged -= OnDamaged;
            _flash.Set(false);
        }

        private void Update()
        {
            // Real time, so the flash still ends during hit stop.
            if (_flash.IsFlashing && Time.unscaledTime >= _flashEndTime)
                _flash.Set(false);
        }

        private void OnDamaged(DamageInfo _)
        {
            _flashEndTime = Time.unscaledTime + _config.HitFlashDuration;
            _flash.Set(true);
        }

        private Renderer[] TargetRenderers() =>
            _renderers != null && _renderers.Length > 0 ? _renderers : GetComponentsInChildren<Renderer>(true);
    }
}
