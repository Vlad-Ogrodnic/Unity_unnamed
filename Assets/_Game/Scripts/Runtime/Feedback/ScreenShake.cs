using UnityEngine;

namespace Game.Feedback
{
    /// <summary>
    /// Lives on the camera. Runs after the camera rig has placed the camera this frame and adds
    /// a fading random offset in the camera's screen plane.
    /// </summary>
    [DefaultExecutionOrder(100)]
    public sealed class ScreenShake : MonoBehaviour
    {
        private float _strength;
        private float _duration;
        private float _endTime;

        public void Shake(float strength, float duration)
        {
            if (strength <= 0f || duration <= 0f)
                return;

            _strength = strength;
            _duration = duration;
            _endTime = Time.unscaledTime + duration;
        }

        private void LateUpdate()
        {
            float remaining = _endTime - Time.unscaledTime;
            if (remaining <= 0f)
                return;

            Vector2 offset = Random.insideUnitCircle * (_strength * remaining / _duration);
            transform.position += transform.right * offset.x + transform.up * offset.y;
        }
    }
}
