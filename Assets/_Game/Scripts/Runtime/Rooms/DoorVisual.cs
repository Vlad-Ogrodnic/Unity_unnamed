using UnityEngine;

namespace Game.Rooms
{
    /// <summary>Lives on a door's <c>Visual</c>: shows a wall piece when sealed, bars when closed, nothing when open.</summary>
    public sealed class DoorVisual : MonoBehaviour
    {
        [SerializeField] private Door _door;
        [SerializeField] private GameObject _wallPiece;
        [SerializeField] private GameObject _bars;

        private void OnEnable()
        {
            _door.StateChanged += Show;
            Show(_door.State);
        }

        private void OnDisable() => _door.StateChanged -= Show;

        private void Show(Door.DoorState state)
        {
            _wallPiece.SetActive(state == Door.DoorState.Sealed);
            _bars.SetActive(state == Door.DoorState.Closed);
        }
    }
}
