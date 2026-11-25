using UnityEngine;

namespace Game.CameraRig
{
    /// <summary>
    /// Orthographic camera that frames the current room (see <see cref="CameraFraming"/>).
    /// Re-frames every frame, so pitch changes in the config and window resizes apply live.
    /// </summary>
    [RequireComponent(typeof(Camera))]
    public sealed class RoomCameraRig : MonoBehaviour
    {
        [SerializeField] private CameraConfig _config;

        [Tooltip("Walkable floor of the starting room; always fully visible.")]
        [SerializeField] private Collider _initialPlayArea;

        [Tooltip("Floor of the starting room including the walls; fills the screen where possible.")]
        [SerializeField] private Collider _initialRoomArea;

        private Camera _camera;
        private Bounds _playArea;
        private Bounds _roomArea;
        private bool _hasRoom;

        private void Awake()
        {
            _camera = GetComponent<Camera>();
            _camera.orthographic = true;

            if (_initialPlayArea != null && _initialRoomArea != null)
                SetRoom(FlattenToBottom(_initialPlayArea.bounds), FlattenToTop(_initialRoomArea.bounds));
        }

        /// <summary>Frames a room. Both areas are boxes on the floor plane (height can be 0).</summary>
        public void SetRoom(Bounds playArea, Bounds roomArea)
        {
            _playArea = playArea;
            _roomArea = roomArea;
            _hasRoom = true;
            Apply();
        }

        private void LateUpdate()
        {
            if (_hasRoom)
                Apply();
        }

        private void Apply()
        {
            CameraFrame frame = CameraFraming.Compute(_playArea, _roomArea, _config.Pitch, _camera.aspect, _config.Distance);
            transform.SetPositionAndRotation(frame.Position, frame.Rotation);
            _camera.orthographicSize = frame.OrthographicSize;
        }

        private static Bounds FlattenToBottom(Bounds b) => Flatten(b, b.min.y);

        private static Bounds FlattenToTop(Bounds b) => Flatten(b, b.max.y);

        private static Bounds Flatten(Bounds b, float y) =>
            new Bounds(new Vector3(b.center.x, y, b.center.z), new Vector3(b.size.x, 0f, b.size.z));
    }
}
