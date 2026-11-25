using System;
using Game.Player;
using NUnit.Framework;

namespace Game.Tests.EditMode
{
    public class InventoryStateTests
    {
        [Test]
        public void StartsEmpty()
        {
            var inventory = new InventoryState(3);
            Assert.That(inventory.Potions, Is.EqualTo(0));
            Assert.That(inventory.HasRoomForPotion, Is.True);
        }

        [Test]
        public void AddPotions_UpToMax()
        {
            var inventory = new InventoryState(3);

            Assert.That(inventory.TryAddPotions(2), Is.True);
            Assert.That(inventory.TryAddPotions(1), Is.True);
            Assert.That(inventory.Potions, Is.EqualTo(3));
            Assert.That(inventory.HasRoomForPotion, Is.False);
        }

        [Test]
        public void AddPotions_ThatDontFit_ChangesNothing()
        {
            var inventory = new InventoryState(3);
            inventory.TryAddPotions(2);

            Assert.That(inventory.TryAddPotions(2), Is.False);
            Assert.That(inventory.Potions, Is.EqualTo(2));
        }

        [Test]
        public void AddPotions_NonPositive_IsRejected()
        {
            var inventory = new InventoryState(3);
            Assert.That(inventory.TryAddPotions(0), Is.False);
            Assert.That(inventory.TryAddPotions(-1), Is.False);
        }

        [Test]
        public void ConsumePotion_OnlyWhenThereIsOne()
        {
            var inventory = new InventoryState(3);
            Assert.That(inventory.TryConsumePotion(), Is.False);

            inventory.TryAddPotions(1);
            Assert.That(inventory.TryConsumePotion(), Is.True);
            Assert.That(inventory.Potions, Is.EqualTo(0));
        }

        [Test]
        public void NegativeMax_Throws()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new InventoryState(-1));
        }
    }
}
