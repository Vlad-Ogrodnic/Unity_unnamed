using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Game.Combat;
using Game.Core;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Game.Tests.PlayMode
{
    public class MeleeTests
    {
        private readonly List<Object> _created = new List<Object>();
        private MeleeAttack _melee;
        private HealthConfig _targetConfig;

        /// <summary>Fixed aim for tests.</summary>
        private sealed class FixedAim : MonoBehaviour, IAimProvider
        {
            public Vector3 AimDirection => Vector3.forward;
            public Vector3 AimPoint => transform.position + Vector3.forward;
        }

        [SetUp]
        public void SetUp()
        {
            var config = ScriptableObject.CreateInstance<MeleeAttackConfig>();
            _created.Add(config);
            _targetConfig = ScriptableObject.CreateInstance<HealthConfig>();
            _created.Add(_targetConfig);
            SetPrivate(_targetConfig, "_maxHealth", 10);

            var ownerConfig = ScriptableObject.CreateInstance<HealthConfig>();
            _created.Add(ownerConfig);

            var attacker = new GameObject("Attacker");
            _created.Add(attacker);
            attacker.SetActive(false);
            var owner = attacker.AddComponent<Health>();
            owner.Configure(ownerConfig, Team.Player);
            var aim = attacker.AddComponent<FixedAim>();
            _melee = attacker.AddComponent<MeleeAttack>();
            SetPrivate(_melee, "_config", config);
            SetPrivate(_melee, "_aimSource", aim);
            SetPrivate(_melee, "_owner", owner);
            attacker.SetActive(true);
        }

        [TearDown]
        public void TearDown()
        {
            foreach (Object obj in _created)
            {
                if (obj != null)
                    Object.Destroy(obj);
            }
            _created.Clear();
        }

        [UnityTest]
        public IEnumerator HitsOnlyEnemiesInsideTheArc()
        {
            Health inFront = Target(Team.Enemy, new Vector3(0f, 0f, 1.2f));
            Health behind = Target(Team.Enemy, new Vector3(0f, 0f, -1.2f));
            Health ally = Target(Team.Player, new Vector3(0.6f, 0f, 1f));
            Health tooFar = Target(Team.Enemy, new Vector3(0f, 0f, 4f));
            yield return new WaitForFixedUpdate();

            int hits = 0;
            _melee.Hit += n => hits = n;
            Assert.That(_melee.TryAttack(), Is.True);

            Assert.That(inFront.Current, Is.EqualTo(7));
            Assert.That(behind.Current, Is.EqualTo(10));
            Assert.That(ally.Current, Is.EqualTo(10));
            Assert.That(tooFar.Current, Is.EqualTo(10));
            Assert.That(hits, Is.EqualTo(1));
        }

        [UnityTest]
        public IEnumerator CooldownBlocksTheNextSwing()
        {
            Health target = Target(Team.Enemy, new Vector3(0f, 0f, 1.2f));
            yield return new WaitForFixedUpdate();

            Assert.That(_melee.TryAttack(), Is.True);
            Assert.That(_melee.TryAttack(), Is.False);
            Assert.That(target.Current, Is.EqualTo(7));

            float end = Time.time + 0.85f;
            while (Time.time < end)
                yield return null;

            Assert.That(_melee.TryAttack(), Is.True);
            Assert.That(target.Current, Is.EqualTo(4));
        }

        private Health Target(Team team, Vector3 position)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            _created.Add(go);
            go.transform.position = position + Vector3.up;
            go.transform.localScale = new Vector3(0.6f, 1f, 0.6f);
            var health = go.AddComponent<Health>();
            health.Configure(_targetConfig, team);
            return health;
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
