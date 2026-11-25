using Game.Core;
using UnityEngine;

namespace Game.Feedback
{
    /// <summary>
    /// Lives on <c>Visual</c>: swaps its renderers to the flash material for a moment when
    /// the root's <see cref="Health"/> takes damage.
    /// </summary>
    public sealed class HitFlash : MonoBehaviour
    {
        [SerializeField] private Health _health;
        [SerializeField] private FeedbackConfig _config;

        private Renderer[] _renderers;
        private Material[][] _originalMaterials;
        private Material[][] _flashMaterials;
        private float _flashEndTime;
        private bool _flashing;

        private void Awake()
        {
            _renderers = GetComponentsInChildren<Renderer>(true);
            _originalMaterials = new Material[_renderers.Length][];
            _flashMaterials = new Material[_renderers.Length][];
            for (int i = 0; i < _renderers.Length; i++)
            {
                _originalMaterials[i] = _renderers[i].sharedMaterials;
                _flashMaterials[i] = new Material[_originalMaterials[i].Length];
                for (int m = 0; m < _flashMaterials[i].Length; m++)
                    _flashMaterials[i][m] = _config.HitFlashMaterial;
            }
        }

        private void OnEnable() => _health.Damaged += OnDamaged;

        private void OnDisable()
        {
            _health.Damaged -= OnDamaged;
            SetFlashing(false);
        }

        private void Update()
        {
            if (_flashing && Time.time >= _flashEndTime)
                SetFlashing(false);
        }

        private void OnDamaged(DamageInfo _)
        {
            _flashEndTime = Time.time + _config.HitFlashDuration;
            SetFlashing(true);
        }

        private void SetFlashing(bool flashing)
        {
            if (_flashing == flashing)
                return;

            _flashing = flashing;
            Material[][] materials = flashing ? _flashMaterials : _originalMaterials;
            for (int i = 0; i < _renderers.Length; i++)
                _renderers[i].sharedMaterials = materials[i];
        }
    }
}
