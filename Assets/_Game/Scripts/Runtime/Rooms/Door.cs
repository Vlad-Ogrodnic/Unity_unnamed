using System;
using UnityEngine;

namespace Game.Rooms
{
    /// <summary>
    /// The gap in a room's wall for one door slot. Unconnected doors stay sealed like a wall.
    /// Connected doors are open, and closed while their room is being fought in.
    /// </summary>
    public sealed class Door : MonoBehaviour
    {
        public enum DoorState
        {
            Sealed,
            Open,
            Closed
        }

        [SerializeField] private DoorSide _side;
        [SerializeField] private Vector2Int _cell;
        [SerializeField] private Collider _blocker;

        [Tooltip("A neighbouring room is behind this door. Set by whoever builds the floor.")]
        [SerializeField] private bool _connected;

        private DoorState _state = DoorState.Sealed;

        /// <summary>For the visual on <c>Visual</c>.</summary>
        public event Action<DoorState> StateChanged;

        public DoorSide Side => _side;
        public Vector2Int Cell => _cell;
        public DoorState State => _state;
        public bool IsConnected => _connected;

        private void Awake()
        {
            _state = _connected ? DoorState.Open : DoorState.Sealed;
            Apply();
        }

        /// <summary>Called by whoever builds the floor when a neighbouring room is behind this door.</summary>
        public void Connect()
        {
            _connected = true;
            SetState(DoorState.Open);
        }

        public void Close()
        {
            if (IsConnected)
                SetState(DoorState.Closed);
        }

        public void Open()
        {
            if (IsConnected)
                SetState(DoorState.Open);
        }

        private void SetState(DoorState state)
        {
            if (_state == state)
                return;
            _state = state;
            Apply();
        }

        private void Apply()
        {
            _blocker.enabled = _state != DoorState.Open;
            StateChanged?.Invoke(_state);
        }
    }
}
