using Game.Player;
using UnityEngine;

namespace Game.Items
{
    /// <summary>
    /// Taken automatically on touch if the player has room; otherwise it stays on the floor
    /// (GDD ch. 3, Pickups). Needs a trigger collider on the Pickup layer.
    /// </summary>
    public sealed class Pickup : MonoBehaviour
    {
        [SerializeField] private PickupKind _kind = PickupKind.Potion;
        [SerializeField, Min(1)] private int _amount = 1;

        // Enter and Stay can both arrive in the same physics step; give the pickup only once.
        private bool _taken;

        // Stay as well as Enter: a full player standing on it takes it as soon as there is room.
        private void OnTriggerEnter(Collider other) => TryGiveTo(other);

        private void OnTriggerStay(Collider other) => TryGiveTo(other);

        private void TryGiveTo(Collider other)
        {
            if (_taken)
                return;

            PlayerInventory inventory = other.GetComponentInParent<PlayerInventory>();
            if (inventory == null || !TryAdd(inventory))
                return;

            _taken = true;
            gameObject.SetActive(false);
            Destroy(gameObject);
        }

        private bool TryAdd(PlayerInventory inventory)
        {
            switch (_kind)
            {
                case PickupKind.Potion:
                    return inventory.TryAddPotions(_amount);
                default:
                    return false;
            }
        }
    }
}
