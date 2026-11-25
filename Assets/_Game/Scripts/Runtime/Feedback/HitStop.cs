using UnityEngine;

namespace Game.Feedback
{
    /// <summary>Freezes game time for a few frames (GDD ch. 4: melee hits, later bomb explosions). One per scene.</summary>
    public sealed class HitStop : MonoBehaviour
    {
        private float _endTime;
        private float _previousTimeScale = 1f;
        private bool _active;

        public void Play(float duration)
        {
            if (duration <= 0f)
                return;

            if (!_active)
            {
                _previousTimeScale = Time.timeScale;
                _active = true;
            }

            _endTime = Mathf.Max(_endTime, Time.unscaledTime + duration);
            Time.timeScale = 0f;
        }

        private void Update()
        {
            if (_active && Time.unscaledTime >= _endTime)
                Stop();
        }

        private void OnDisable()
        {
            if (_active)
                Stop();
        }

        private void Stop()
        {
            _active = false;
            Time.timeScale = _previousTimeScale;
        }
    }
}
