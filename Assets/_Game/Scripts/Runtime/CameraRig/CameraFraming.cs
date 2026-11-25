using UnityEngine;

namespace Game.CameraRig
{
    /// <summary>Where an orthographic camera must be to frame a room.</summary>
    public readonly struct CameraFrame
    {
        public readonly Vector3 Position;
        public readonly Quaternion Rotation;
        public readonly float OrthographicSize;

        public CameraFrame(Vector3 position, Quaternion rotation, float orthographicSize)
        {
            Position = position;
            Rotation = rotation;
            OrthographicSize = orthographicSize;
        }
    }

    /// <summary>
    /// Pure framing math for a room seen by an orthographic camera tilted around X.
    /// Rule: the play area (walkable floor) is always fully visible; within that, the
    /// room (floor including walls) fills the screen, so only the outer edge of the walls
    /// gets cropped. Only when the screen's shape can't satisfy both does background show.
    /// </summary>
    public static class CameraFraming
    {
        public static CameraFrame Compute(Bounds playArea, Bounds roomArea, float pitchDegrees, float aspect, float distance)
        {
            Quaternion rotation = Quaternion.Euler(pitchDegrees, 0f, 0f);
            Vector3 right = rotation * Vector3.right;
            Vector3 up = rotation * Vector3.up;
            Vector3 forward = rotation * Vector3.forward;

            Project(playArea, right, up, out Vector2 playMin, out Vector2 playMax);
            Project(roomArea, right, up, out Vector2 roomMin, out Vector2 roomMax);

            // Smallest size that contains the play area, largest that the room still covers.
            float containPlay = Mathf.Max((playMax.y - playMin.y) * 0.5f, (playMax.x - playMin.x) * 0.5f / aspect);
            float coverRoom = Mathf.Min((roomMax.y - roomMin.y) * 0.5f, (roomMax.x - roomMin.x) * 0.5f / aspect);
            float size = Mathf.Max(containPlay, coverRoom);
            float halfWidth = size * aspect;

            // Centre on the room, shifted only as needed to keep the play area on screen.
            Vector2 centre = (roomMin + roomMax) * 0.5f;
            centre.x = Mathf.Clamp(centre.x, playMax.x - halfWidth, playMin.x + halfWidth);
            centre.y = Mathf.Clamp(centre.y, playMax.y - size, playMin.y + size);

            Vector3 lookAt = right * centre.x + up * centre.y + forward * Vector3.Dot(roomArea.center, forward);
            return new CameraFrame(lookAt - forward * distance, rotation, size);
        }

        /// <summary>
        /// Framing for a room bigger than the screen: the view keeps <paramref name="orthographicSize"/>,
        /// centres on <paramref name="focus"/> (the player) and is clamped so it never shows past the
        /// room's edges. On an axis where the room is smaller than the view, it stays centred on the room.
        /// </summary>
        public static CameraFrame ComputeFollow(Bounds roomArea, Vector3 focus, float orthographicSize, float pitchDegrees, float aspect, float distance)
        {
            Quaternion rotation = Quaternion.Euler(pitchDegrees, 0f, 0f);
            Vector3 right = rotation * Vector3.right;
            Vector3 up = rotation * Vector3.up;
            Vector3 forward = rotation * Vector3.forward;

            Project(roomArea, right, up, out Vector2 roomMin, out Vector2 roomMax);
            float halfWidth = orthographicSize * aspect;
            var centre = new Vector2(Vector3.Dot(focus, right), Vector3.Dot(focus, up));
            centre.x = ClampOrMid(centre.x, roomMin.x + halfWidth, roomMax.x - halfWidth);
            centre.y = ClampOrMid(centre.y, roomMin.y + orthographicSize, roomMax.y - orthographicSize);

            Vector3 lookAt = right * centre.x + up * centre.y + forward * Vector3.Dot(roomArea.center, forward);
            return new CameraFrame(lookAt - forward * distance, rotation, orthographicSize);
        }

        private static float ClampOrMid(float value, float min, float max) =>
            min <= max ? Mathf.Clamp(value, min, max) : (min + max) * 0.5f;

        /// <summary>Extents of a box's 8 corners along the camera's right (x) and up (y) axes.</summary>
        private static void Project(Bounds bounds, Vector3 right, Vector3 up, out Vector2 min, out Vector2 max)
        {
            min = new Vector2(float.PositiveInfinity, float.PositiveInfinity);
            max = new Vector2(float.NegativeInfinity, float.NegativeInfinity);
            Vector3 c = bounds.center;
            Vector3 e = bounds.extents;
            for (int i = 0; i < 8; i++)
            {
                Vector3 corner = c + new Vector3(
                    (i & 1) == 0 ? -e.x : e.x,
                    (i & 2) == 0 ? -e.y : e.y,
                    (i & 4) == 0 ? -e.z : e.z);
                var p = new Vector2(Vector3.Dot(corner, right), Vector3.Dot(corner, up));
                min = Vector2.Min(min, p);
                max = Vector2.Max(max, p);
            }
        }
    }
}
