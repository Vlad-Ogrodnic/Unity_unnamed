using Game.Core;
using NUnit.Framework;
using UnityEngine;

namespace Game.Tests.EditMode
{
    /// <summary>
    /// The aim chain used by PlayerAim: cursor ray -> floor plane -> direction from the player.
    /// Checked at the camera angles the GDD allows (60-90 degrees) and in every screen corner.
    /// </summary>
    public class AimTests
    {
        private Camera _camera;

        [SetUp]
        public void SetUp()
        {
            _camera = new GameObject("AimTestCamera").AddComponent<Camera>();
            _camera.orthographic = true;
            _camera.orthographicSize = 6f;
            _camera.aspect = 16f / 9f;
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(_camera.gameObject);
        }

        [Test]
        public void CursorRay_HitsTheFloorPointUnderIt_AtAnyAllowedAngleAndCorner(
            [Values(60f, 75f, 90f)] float pitch,
            [Values(0.02f, 0.5f, 0.98f)] float viewportX,
            [Values(0.02f, 0.5f, 0.98f)] float viewportY)
        {
            _camera.transform.rotation = Quaternion.Euler(pitch, 0f, 0f);
            _camera.transform.position = -_camera.transform.forward * 30f;

            Ray ray = _camera.ViewportPointToRay(new Vector3(viewportX, viewportY, 0f));
            bool hit = PlanarMath.TryIntersectHorizontalPlane(ray, 0f, out Vector3 point);

            Assert.That(hit, Is.True);
            Assert.That(point.y, Is.EqualTo(0f));
            Vector3 back = _camera.WorldToViewportPoint(point);
            Assert.That(back.x, Is.EqualTo(viewportX).Within(1e-3f));
            Assert.That(back.y, Is.EqualTo(viewportY).Within(1e-3f));
        }

        [Test]
        public void Direction_IsFlatNormalizedAndPointsAtTarget()
        {
            Vector3 direction = PlanarMath.Direction(new Vector3(1f, 0f, 1f), new Vector3(4f, 2f, 5f), Vector3.forward);

            Assert.That(direction.y, Is.EqualTo(0f));
            Assert.That(direction.magnitude, Is.EqualTo(1f).Within(1e-5f));
            Assert.That(direction.x, Is.EqualTo(0.6f).Within(1e-5f));
            Assert.That(direction.z, Is.EqualTo(0.8f).Within(1e-5f));
        }

        [Test]
        public void Direction_SamePoint_ReturnsFallback()
        {
            Assert.That(PlanarMath.Direction(Vector3.one, Vector3.one, Vector3.left), Is.EqualTo(Vector3.left));
        }

        [Test]
        public void RayParallelOrPointingAway_DoesNotHit()
        {
            Assert.That(PlanarMath.TryIntersectHorizontalPlane(new Ray(Vector3.up, Vector3.forward), 0f, out _), Is.False);
            Assert.That(PlanarMath.TryIntersectHorizontalPlane(new Ray(Vector3.up, Vector3.up), 0f, out _), Is.False);
        }
    }
}
