using Game.Enemies;
using UnityEngine;

namespace Game.Feedback
{
    /// <summary>Lives on a charger's <c>Visual</c>: blinks white while the charger telegraphs its charge.</summary>
    public sealed class TelegraphBlink : MonoBehaviour
    {
        [SerializeField] private ChargerBehaviour _charger;
        [SerializeField] private FeedbackConfig _config;
        [Tooltip("Renderers to affect. Empty = every renderer under this object.")]
        [SerializeField] private Renderer[] _renderers = System.Array.Empty<Renderer>();

        private RendererFlash _flash;
        private bool _blinking;
        private float _startTime;

        private void Awake()
        {
            _flash = new RendererFlash(TargetRenderers(), _config.HitFlashMaterial);
        }

        private void OnEnable()
        {
            _charger.TelegraphStarted += OnStarted;
            _charger.TelegraphEnded += OnEnded;
        }

        private void OnDisable()
        {
            _charger.TelegraphStarted -= OnStarted;
            _charger.TelegraphEnded -= OnEnded;
            OnEnded();
        }

        private void Update()
        {
            if (!_blinking)
                return;

            float interval = _config.TelegraphBlinkInterval;
            _flash.Set(Mathf.Repeat(Time.time - _startTime, 2f * interval) < interval);
        }

        private void OnStarted(Vector3 _)
        {
            _blinking = true;
            _startTime = Time.time;
        }

        private void OnEnded()
        {
            _blinking = false;
            _flash.Set(false);
        }

        private Renderer[] TargetRenderers() =>
            _renderers != null && _renderers.Length > 0 ? _renderers : GetComponentsInChildren<Renderer>(true);
    }
}
