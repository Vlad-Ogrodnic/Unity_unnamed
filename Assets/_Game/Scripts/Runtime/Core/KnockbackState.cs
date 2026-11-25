using UnityEngine;

namespace Game.Core
{
    /// <summary>
    /// A push that fades out linearly. Movers read <see cref="Velocity"/> instead of their own
    /// movement while <see cref="IsActive"/>. A new push replaces the current one.
    /// </summary>
    public sealed class KnockbackState
    {
        private Vector3 _initialVelocity;
        private float _duration;
        private float _remaining;

        public bool IsActive => _remaining > 0f;
        public Vector3 Velocity => IsActive ? _initialVelocity * (_remaining / _duration) : Vector3.zero;

        /// <param name="direction">Push direction; flattened to XZ.</param>
        /// <param name="force">Initial push speed in m/s.</param>
        /// <param name="duration">Seconds until the push has faded out.</param>
        public void Apply(Vector3 direction, float force, float duration)
        {
            Vector3 flat = PlanarMath.FlattenAndNormalize(direction);
            if (force <= 0f || duration <= 0f || flat == Vector3.zero)
                return;

            _initialVelocity = flat * force;
            _duration = duration;
            _remaining = duration;
        }

        public void Tick(float deltaTime)
        {
            _remaining = Mathf.Max(0f, _remaining - deltaTime);
        }

        public void Clear()
        {
            _remaining = 0f;
        }
    }
}
