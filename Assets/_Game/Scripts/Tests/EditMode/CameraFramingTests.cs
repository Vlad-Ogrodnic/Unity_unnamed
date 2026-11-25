using Game.CameraRig;
using Game.Core;
using NUnit.Framework;
using UnityEngine;

namespace Game.Tests.EditMode
{
    /// <summary>
    /// Checks frames with plain orthographic projection math instead of a Camera component,
    /// whose projection depends on the Game view size and would make the results editor-dependent.
    /// </summary>
    public class CameraFramingTests
    {
        private const float Tolerance = 1e-3f;

        // Same shape as Sandbox_Template: 18x10 m walkable floor, 1 m walls around it.
        private static readonly Bounds PlayArea = new Bounds(Vector3.zero, new Vector3(18f, 0f, 10f));
        private static readonly Bounds RoomArea = new Bounds(Vector3.zero, new Vector3(20f, 0f, 12f));

        private static readonly float[] Aspects = { 16f / 9f, 16f / 10f, 4f / 3f, 21f / 9f };

        [Test]
        public void PlayArea_IsFullyVisible(
            [Values(60f, 75f, 90f)] float pitch,
            [ValueSource(nameof(Aspects))] float aspect)
        {
            CameraFrame frame = CameraFraming.Compute(PlayArea, RoomArea, pitch, aspect, 30f);

            foreach (Vector3 corner in Corners(PlayArea))
            {
                Vector3 local = ToCameraSpace(frame, corner);
                Assert.That(Mathf.Abs(local.x), Is.LessThanOrEqualTo(frame.OrthographicSize * aspect + Tolerance), $"corner {corner} x");
                Assert.That(Mathf.Abs(local.y), Is.LessThanOrEqualTo(frame.OrthographicSize + Tolerance), $"corner {corner} y");
                Assert.That(local.z, Is.GreaterThan(0f), $"corner {corner} in front of the camera");
            }
        }

        [Test]
        public void At16By9_RoomFillsTheScreen([Values(60f, 75f, 90f)] float pitch)
        {
            const float aspect = 16f / 9f;
            CameraFrame frame = CameraFraming.Compute(PlayArea, RoomArea, pitch, aspect, 30f);
            float halfHeight = frame.OrthographicSize;
            float halfWidth = halfHeight * aspect;

            // Every screen corner looks at a point inside the room's floor rectangle.
            foreach (Vector2 corner in new[] { new Vector2(-1f, -1f), new Vector2(1f, -1f), new Vector2(-1f, 1f), new Vector2(1f, 1f) })
            {
                Vector3 origin = frame.Position + frame.Rotation * new Vector3(corner.x * halfWidth, corner.y * halfHeight, 0f);
                var ray = new Ray(origin, frame.Rotation * Vector3.forward);
                Assert.That(PlanarMath.TryIntersectHorizontalPlane(ray, 0f, out Vector3 point), Is.True);
                Assert.That(Mathf.Abs(point.x), Is.LessThanOrEqualTo(RoomArea.extents.x + Tolerance), $"screen corner {corner}");
                Assert.That(Mathf.Abs(point.z), Is.LessThanOrEqualTo(RoomArea.extents.z + Tolerance), $"screen corner {corner}");
            }
        }

        [Test]
        public void BigRoom_FollowsThePlayer_ButNeverShowsPastTheWalls()
        {
            const float aspect = 16f / 9f;
            var bigRoom = new Bounds(Vector3.zero, new Vector3(40f, 0f, 24f)); // 2x2 cells
            float size = CameraFraming.Compute(PlayArea, RoomArea, 75f, aspect, 30f).OrthographicSize;

            // Player in the middle: view centred on the player.
            CameraFrame middle = CameraFraming.ComputeFollow(bigRoom, Vector3.zero, size, 75f, aspect, 30f);
            Assert.That(Mathf.Abs(ToCameraSpace(middle, Vector3.zero).x), Is.LessThan(Tolerance));

            // Player in a corner: the view stops at the walls.
            CameraFrame corner = CameraFraming.ComputeFollow(bigRoom, new Vector3(19f, 0f, 11f), size, 75f, aspect, 30f);
            foreach (Vector2 c in new[] { new Vector2(-1f, -1f), new Vector2(1f, -1f), new Vector2(-1f, 1f), new Vector2(1f, 1f) })
            {
                Vector3 origin = corner.Position + corner.Rotation * new Vector3(c.x * size * aspect, c.y * size, 0f);
                PlanarMath.TryIntersectHorizontalPlane(new Ray(origin, corner.Rotation * Vector3.forward), 0f, out Vector3 p);
                Assert.That(Mathf.Abs(p.x), Is.LessThanOrEqualTo(bigRoom.extents.x + Tolerance));
                Assert.That(Mathf.Abs(p.z), Is.LessThanOrEqualTo(bigRoom.extents.z + Tolerance));
            }
        }

        [Test]
        public void OffCentreRoom_IsFramedWhereItIs()
        {
            var offset = new Vector3(40f, 0f, -25f);
            var play = new Bounds(PlayArea.center + offset, PlayArea.size);
            var room = new Bounds(RoomArea.center + offset, RoomArea.size);
            CameraFrame centred = CameraFraming.Compute(PlayArea, RoomArea, 75f, 16f / 9f, 30f);
            CameraFrame moved = CameraFraming.Compute(play, room, 75f, 16f / 9f, 30f);

            Assert.That(moved.OrthographicSize, Is.EqualTo(centred.OrthographicSize).Within(1e-4f));
            Assert.That(Vector3.Distance(moved.Position, centred.Position + offset), Is.LessThan(1e-3f));
        }

        private static Vector3 ToCameraSpace(CameraFrame frame, Vector3 world) =>
            Quaternion.Inverse(frame.Rotation) * (world - frame.Position);

        private static Vector3[] Corners(Bounds b) => new[]
        {
            new Vector3(b.min.x, b.min.y, b.min.z), new Vector3(b.max.x, b.min.y, b.min.z),
            new Vector3(b.min.x, b.min.y, b.max.z), new Vector3(b.max.x, b.min.y, b.max.z),
        };
    }
}
