using System;
using UnityEngine;

namespace Game.Player
{
    /// <summary>The player's consumables. UI and feedback listen to <see cref="Changed"/>.</summary>
    public sealed class PlayerInventory : MonoBehaviour
    {
        [SerializeField] private PlayerConfig _config;

        private InventoryState _state;

        public event Action Changed;

        public int Potions => _state.Potions;
        public int MaxPotions => _state.MaxPotions;

        private void Awake()
        {
            _state = new InventoryState(_config.MaxPotions);
        }

        public bool TryAddPotions(int amount)
        {
            if (!_state.TryAddPotions(amount))
                return false;

            Changed?.Invoke();
            return true;
        }

        public bool TryConsumePotion()
        {
            if (!_state.TryConsumePotion())
                return false;

            Changed?.Invoke();
            return true;
        }
    }
}
