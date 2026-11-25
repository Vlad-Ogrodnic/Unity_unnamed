using System.Collections;
using System.Reflection;
using Game.Core;
using Game.Player;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Game.Tests.PlayMode
{
    public class PotionTests
    {
        private GameObject _player;
        private PlayerConfig _config;
        private Health _health;
        private PlayerInventory _inventory;
        private PlayerPotionUse _potionUse;

        [SetUp]
        public void SetUp()
        {
            _config = ScriptableObject.CreateInstance<PlayerConfig>();
            SetPrivate(_config, "_maxHealth", 6);

            _player = new GameObject("Player");
            _player.SetActive(false);
            _health = _player.AddComponent<Health>();
            _health.Configure(_config, Team.Player);
            _inventory = _player.AddComponent<PlayerInventory>();
            SetPrivate(_inventory, "_config", _config);
            _potionUse = _player.AddComponent<PlayerPotionUse>();
            SetPrivate(_potionUse, "_config", _config);
            SetPrivate(_potionUse, "_inventory", _inventory);
            SetPrivate(_potionUse, "_health", _health);
            _player.SetActive(true);
        }

        [TearDown]
        public void TearDown()
        {
            Object.Destroy(_player);
            Object.Destroy(_config);
        }

        [UnityTest]
        public IEnumerator FullHealth_DoesNotDrink_AndSignalsFailure()
        {
            _inventory.TryAddPotions(1);
            int failures = 0;
            _potionUse.UseFailed += () => failures++;
            yield return null;

            Assert.That(_potionUse.TryDrink(), Is.False);
            Assert.That(_inventory.Potions, Is.EqualTo(1));
            Assert.That(failures, Is.EqualTo(1));
        }

        [UnityTest]
        public IEnumerator Drinking_HealsTwo_WithoutGoingOverMax()
        {
            _inventory.TryAddPotions(2);
            _health.TryTakeDamage(new DamageInfo(3, Vector3.zero, 0f, null));
            yield return null;

            Assert.That(_potionUse.TryDrink(), Is.True);
            Assert.That(_health.Current, Is.EqualTo(5));
            Assert.That(_potionUse.TryDrink(), Is.True);
            Assert.That(_health.Current, Is.EqualTo(6));
            Assert.That(_inventory.Potions, Is.EqualTo(0));
        }

        [UnityTest]
        public IEnumerator NoPotions_SignalsFailure()
        {
            _health.TryTakeDamage(new DamageInfo(1, Vector3.zero, 0f, null));
            int failures = 0;
            _potionUse.UseFailed += () => failures++;
            yield return null;

            Assert.That(_potionUse.TryDrink(), Is.False);
            Assert.That(failures, Is.EqualTo(1));
        }

        private static void SetPrivate(object target, string field, object value)
        {
            System.Type type = target.GetType();
            FieldInfo info = null;
            while (info == null && type != null)
            {
                info = type.GetField(field, BindingFlags.Instance | BindingFlags.NonPublic);
                type = type.BaseType;
            }
            Assert.That(info, Is.Not.Null, field);
            info.SetValue(target, value);
        }
    }
}
