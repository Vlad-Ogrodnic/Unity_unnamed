using UnityEngine;

namespace Game.Core
{
    /// <summary>When <see cref="Health"/> dies: turns off the listed behaviours and optionally the whole object.</summary>
    public sealed class DisableOnDeath : MonoBehaviour
    {
        [SerializeField] private Health _health;
        [SerializeField] private Behaviour[] _behaviours = System.Array.Empty<Behaviour>();
        [SerializeField] private bool _deactivateGameObject;

        private void OnEnable() => _health.Died += OnDied;

        private void OnDisable() => _health.Died -= OnDied;

        private void OnDied()
        {
            foreach (Behaviour behaviour in _behaviours)
            {
                if (behaviour != null)
                    behaviour.enabled = false;
            }

            if (_deactivateGameObject)
                gameObject.SetActive(false);
        }
    }
}
