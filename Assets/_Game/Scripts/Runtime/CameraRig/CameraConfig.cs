using UnityEngine;

namespace Game.CameraRig
{
    /// <summary>Camera values. GDD ch. 1, Camera: orthographic, 75 degrees by default, adjustable 60-90.</summary>
    [CreateAssetMenu(menuName = "Game/Camera Config", fileName = "CameraConfig")]
    public sealed class CameraConfig : ScriptableObject
    {
        [Tooltip("Tilt from the horizontal, in degrees. 90 = straight down.")]
        [SerializeField, Range(60f, 90f)] private float _pitch = 75f;

        [Tooltip("Distance from the room to the camera. Orthographic, so it only matters for clipping.")]
        [SerializeField, Min(1f)] private float _distance = 30f;

        [Tooltip("Seconds the camera takes to move to the next room (GDD: smooth transition).")]
        [SerializeField, Min(0f)] private float _roomTransitionDuration = 0.35f;

        public float Pitch => _pitch;
        public float RoomTransitionDuration => _roomTransitionDuration;
        public float Distance => _distance;
    }
}
