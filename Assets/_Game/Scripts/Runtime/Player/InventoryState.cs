using System;

namespace Game.Player
{
    /// <summary>What the player carries, as plain logic. Starts empty (GDD decision). Coins come with M4-02.</summary>
    public sealed class InventoryState
    {
        public int Potions { get; private set; }
        public int MaxPotions { get; }

        public InventoryState(int maxPotions)
        {
            if (maxPotions < 0)
                throw new ArgumentOutOfRangeException(nameof(maxPotions));
            MaxPotions = maxPotions;
        }

        public bool HasRoomForPotion => Potions < MaxPotions;

        /// <summary>Adds potions only if all of them fit; otherwise nothing changes (the pickup stays on the floor).</summary>
        public bool TryAddPotions(int amount)
        {
            if (amount <= 0 || Potions + amount > MaxPotions)
                return false;

            Potions += amount;
            return true;
        }

        public bool TryConsumePotion()
        {
            if (Potions <= 0)
                return false;

            Potions--;
            return true;
        }
    }
}
