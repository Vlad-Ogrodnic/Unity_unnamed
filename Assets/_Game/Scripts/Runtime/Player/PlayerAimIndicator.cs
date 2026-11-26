using UnityEngine;

namespace Game.Player
{
    /// <summary>Visual only: turns this transform (inside <c>Visual</c>) to face the aim direction.</summary>
    public sealed class PlayerAimIndicator : MonoBehaviour
    {
        [SerializeField] private PlayerAim _aim;

        private void LateUpdate()
        {
            transform.rotation = Quaternion.LookRotation(_aim.AimDirection, Vector3.up);
        }
    }
}
