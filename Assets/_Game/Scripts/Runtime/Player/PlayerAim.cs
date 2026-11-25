using Game.Core;
using UnityEngine;

namespace Game.Player
{
    /// <summary>Aims at the point of the floor plane under the mouse cursor.</summary>
    [DefaultExecutionOrder(-50)]
    public sealed class PlayerAim : MonoBehaviour, IAimProvider
    {
        [SerializeField] private PlayerInputReader _input;

        [Tooltip("Camera the cursor is read through. Set by whoever spawns the player, or in the scene.")]
        [SerializeField] private Camera _viewCamera;

        public Vector3 AimDirection { get; private set; } = Vector3.forward;
        public Vector3 AimPoint { get; private set; }

        public void SetCamera(Camera viewCamera)
        {
            _viewCamera = viewCamera;
        }

        private void Update()
        {
            if (_viewCamera == null)
                return;

            Vector3 origin = transform.position;
            Ray ray = _viewCamera.ScreenPointToRay(_input.AimScreenPosition);
            if (!PlanarMath.TryIntersectHorizontalPlane(ray, origin.y, out Vector3 point))
                return;

            AimPoint = point;
            AimDirection = PlanarMath.Direction(origin, point, AimDirection);
        }
    }
}
