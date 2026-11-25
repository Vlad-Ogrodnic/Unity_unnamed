using System;
using Game.Core;
using UnityEngine;

namespace Game.Player
{
    /// <summary>
    /// Drinks a potion on the Potion key: instant heal, no slowdown (GDD ch. 3).
    /// With full HP or no potions nothing is used and <see cref="UseFailed"/> fires, for a visual hint.
    /// </summary>
    public sealed class PlayerPotionUse : MonoBehaviour
    {
        [SerializeField] private PlayerConfig _config;
        [SerializeField] private PlayerInputReader _input;
        [SerializeField] private PlayerInventory _inventory;
        [SerializeField] private Health _health;

        public event Action UseFailed;

        private void OnEnable()
        {
            if (_input != null)
                _input.PotionPressed += OnPotionPressed;
        }

        private void OnDisable()
        {
            if (_input != null)
                _input.PotionPressed -= OnPotionPressed;
        }

        /// <summary>Drinks a potion if HP isn't full and one is available. Returns whether it was drunk.</summary>
        public bool TryDrink()
        {
            if (_health.IsDead)
                return false;

            if (_health.Current >= _health.Max || !_inventory.TryConsumePotion())
            {
                UseFailed?.Invoke();
                return false;
            }

            _health.Heal(_config.PotionHealAmount);
            return true;
        }

        private void OnPotionPressed() => TryDrink();
    }
}
