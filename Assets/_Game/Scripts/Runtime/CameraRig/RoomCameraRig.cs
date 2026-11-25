using Game.Rooms;
using UnityEngine;

namespace Game.CameraRig
{
    /// <summary>
    /// Orthographic camera that frames the current room (see <see cref="CameraFraming"/>).
    /// A room that fits on screen is framed whole; a bigger room (2×1, 1×2, 2×2) is seen at the
    /// size of a 1×1 room and the camera follows the player inside its walls. Moving to another
    /// room is a smooth transition. Re-frames every frame, so config and window changes apply live.
    /// </summary>
    [RequireComponent(typeof(Camera))]
    public sealed class RoomCameraRig : MonoBehaviour
    {
        [SerializeField] private CameraConfig _config;

        [Tooltip("Rooms placed by hand in this scene; generated floors register theirs with Register().")]
        [SerializeField] private Room[] _rooms = System.Array.Empty<Room>();

        [Header("Sandbox scenes without Room components")]
        [Tooltip("Walkable floor of the starting room; always fully visible.")]
        [SerializeField] private Collider _initialPlayArea;

        [Tooltip("Floor of the starting room including the walls; fills the screen where possible.")]
        [SerializeField] private Collider _initialRoomArea;

        private Camera _camera;
        private Bounds _playArea;
        private Bounds _roomArea;
        private Transform _focus;
        private bool _hasRoom;

        private CameraFrame _transitionFrom;
        private float _transitionTime;
        private bool _transitioning;

        private void Awake()
        {
            _camera = GetComponent<Camera>();
            _camera.orthographic = true;

            foreach (Room room in _rooms)
                Register(room);

            if (_initialPlayArea != null && _initialRoomArea != null)
                SetRoom(FlattenToBottom(_initialPlayArea.bounds), FlattenToTop(_initialRoomArea.bounds), null, false);
        }

        private void OnDestroy()
        {
            foreach (Room room in _rooms)
            {
                if (room != null)
                    room.PlayerEntered -= OnPlayerEntered;
            }
        }

        public void Register(Room room)
        {
            room.PlayerEntered -= OnPlayerEntered;
            room.PlayerEntered += OnPlayerEntered;
        }

        /// <summary>Frames a room. Both areas are boxes on the floor plane; <paramref name="focus"/> is followed in big rooms.</summary>
        public void SetRoom(Bounds playArea, Bounds roomArea, Transform focus, bool transition)
        {
            if (transition && _hasRoom && _config.RoomTransitionDuration > 0f)
            {
                _transitionFrom = new CameraFrame(transform.position, transform.rotation, _camera.orthographicSize);
                _transitionTime = 0f;
                _transitioning = true;
            }

            _playArea = playArea;
            _roomArea = roomArea;
            _focus = focus;
            _hasRoom = true;
            Apply(Time.deltaTime);
        }

        private void OnPlayerEntered(Room room, Transform player)
        {
            bool sameRoom = _hasRoom && room.RoomArea.center == _roomArea.center;
            if (!sameRoom)
                SetRoom(room.PlayArea, room.RoomArea, player, true);
        }

        private void LateUpdate()
        {
            if (_hasRoom)
                Apply(Time.deltaTime);
        }

        private void Apply(float deltaTime)
        {
            CameraFrame target = TargetFrame();
            if (_transitioning)
            {
                _transitionTime += deltaTime;
                float t = Mathf.SmoothStep(0f, 1f, _transitionTime / _config.RoomTransitionDuration);
                target = new CameraFrame(
                    Vector3.Lerp(_transitionFrom.Position, target.Position, t),
                    Quaternion.Slerp(_transitionFrom.Rotation, target.Rotation, t),
                    Mathf.Lerp(_transitionFrom.OrthographicSize, target.OrthographicSize, t));
                _transitioning = t < 1f;
            }

            transform.SetPositionAndRotation(target.Position, target.Rotation);
            _camera.orthographicSize = target.OrthographicSize;
        }

        private CameraFrame TargetFrame()
        {
            float aspect = _camera.aspect;
            CameraFrame whole = CameraFraming.Compute(_playArea, _roomArea, _config.Pitch, aspect, _config.Distance);
            if (_focus == null)
                return whole;

            // A room bigger than one cell is viewed at the scale of a 1x1 room, following the player.
            Vector2Int interior = RoomGrid.InteriorSize(Vector2Int.one);
            Vector2Int outer = RoomGrid.OuterSize(Vector2Int.one);
            var cellPlay = new Bounds(Vector3.zero, new Vector3(interior.x, 0f, interior.y));
            var cellRoom = new Bounds(Vector3.zero, new Vector3(outer.x, 0f, outer.y));
            float cellSize = CameraFraming.Compute(cellPlay, cellRoom, _config.Pitch, aspect, _config.Distance).OrthographicSize;
            if (whole.OrthographicSize <= cellSize + 0.01f)
                return whole;

            return CameraFraming.ComputeFollow(_roomArea, _focus.position, cellSize, _config.Pitch, aspect, _config.Distance);
        }

        private static Bounds FlattenToBottom(Bounds b) => Flatten(b, b.min.y);

        private static Bounds FlattenToTop(Bounds b) => Flatten(b, b.max.y);

        private static Bounds Flatten(Bounds b, float y) =>
            new Bounds(new Vector3(b.center.x, y, b.center.z), new Vector3(b.size.x, 0f, b.size.z));
    }
}
