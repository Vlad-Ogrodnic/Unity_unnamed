using UnityEngine;

namespace Game.Core
{
    /// <summary>Helpers for gameplay on the XZ plane (Y is up and ignored for directions).</summary>
    public static class PlanarMath
    {
        private const float Epsilon = 1e-6f;

        public static Vector3 Flatten(Vector3 vector)
        {
            vector.y = 0f;
            return vector;
        }

        /// <summary>Projects onto the XZ plane and normalizes; zero for a vertical or zero vector.</summary>
        public static Vector3 FlattenAndNormalize(Vector3 vector)
        {
            vector.y = 0f;
            return vector.sqrMagnitude > Epsilon ? vector.normalized : Vector3.zero;
        }

        /// <summary>Normalized XZ direction from one point to another, or <paramref name="fallback"/> if they coincide on XZ.</summary>
        public static Vector3 Direction(Vector3 from, Vector3 to, Vector3 fallback)
        {
            Vector3 delta = to - from;
            delta.y = 0f;
            return delta.sqrMagnitude > Epsilon ? delta.normalized : fallback;
        }

        /// <summary>Where a ray crosses the horizontal plane at height <paramref name="planeY"/>, if it does in front of its origin.</summary>
        public static bool TryIntersectHorizontalPlane(Ray ray, float planeY, out Vector3 point)
        {
            float dirY = ray.direction.y;
            if (Mathf.Abs(dirY) < Epsilon)
            {
                point = default;
                return false;
            }

            float distance = (planeY - ray.origin.y) / dirY;
            if (distance < 0f)
            {
                point = default;
                return false;
            }

            point = ray.origin + ray.direction * distance;
            point.y = planeY;
            return true;
        }
    }
}
