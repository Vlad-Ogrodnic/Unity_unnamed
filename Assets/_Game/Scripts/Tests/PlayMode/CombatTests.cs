using System.Collections;
using Game.Combat;
using Game.Core;
using Game.Enemies;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Game.Tests.PlayMode
{
    /// <summary>Projectiles and contact damage against real colliders, using the project's layers.</summary>
    public class CombatTests
    {
        private readonly System.Collections.Generic.List<Object> _created = new System.Collections.Generic.List<Object>();

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
        public IEnumerator PlayerProjectile_DamagesEnemyOnce_AndDisappears()
        {
            Health enemy = CreateTarget("Enemy", Team.Enemy, new Vector3(0f, 0f, 4f), maxHealth: 3);
            Projectile projectile = CreateProjectile("PlayerProjectile");
            bool released = false;

            projectile.Launch(Vector3.forward, Stats(), null, p => released = true);
            yield return WaitSeconds(1f);

            Assert.That(enemy.Current, Is.EqualTo(2));
            Assert.That(released, Is.True);
        }

        [UnityTest]
        public IEnumerator PlayerProjectile_PassesThroughPlayer()
        {
            Health player = CreateTarget("Player", Team.Player, new Vector3(0f, 0f, 3f), maxHealth: 6);
            Projectile projectile = CreateProjectile("PlayerProjectile");

            projectile.Launch(Vector3.forward, Stats(), null, null);
            yield return WaitSeconds(1f);

            Assert.That(player.Current, Is.EqualTo(6));
        }

        [UnityTest]
        public IEnumerator Projectile_StopsAtObstacle_AndDisappearsAfterRange()
        {
            var wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
            _created.Add(wall);
            wall.layer = LayerMask.NameToLayer("Obstacle");
            wall.transform.position = new Vector3(0f, 0.5f, 3f);
            Health enemyBehindWall = CreateTarget("Enemy", Team.Enemy, new Vector3(0f, 0f, 5f), maxHealth: 3);

            Projectile blocked = CreateProjectile("PlayerProjectile");
            bool blockedReleased = false;
            blocked.Launch(Vector3.forward, Stats(), null, _ => blockedReleased = true);

            Projectile shortRange = CreateProjectile("PlayerProjectile");
            shortRange.transform.position = new Vector3(10f, 0.5f, 0f);
            bool rangeReleased = false;
            shortRange.Launch(Vector3.forward, new ProjectileStats(1, 20f, 2f, 0.15f, 0f), null, _ => rangeReleased = true);

            yield return WaitSeconds(1f);

            Assert.That(enemyBehindWall.Current, Is.EqualTo(3));
            Assert.That(blockedReleased, Is.True);
            Assert.That(rangeReleased, Is.True);
            Assert.That(shortRange.transform.position.z, Is.EqualTo(2f).Within(0.01f));
        }

        [UnityTest]
        public IEnumerator Health_DiedFiresOnce()
        {
            Health target = CreateTarget("Enemy", Team.Enemy, Vector3.zero, maxHealth: 1);
            int deaths = 0;
            target.Died += () => deaths++;

            target.TryTakeDamage(new DamageInfo(1, Vector3.zero, 0f, null));
            target.TryTakeDamage(new DamageInfo(1, Vector3.zero, 0f, null));
            yield return null;

            Assert.That(deaths, Is.EqualTo(1));
            Assert.That(target.IsDead, Is.True);
        }

        [UnityTest]
        public IEnumerator ContactDamage_HurtsPlayer_ThenIFramesBlockRepeats()
        {
            var playerConfig = ScriptableObject.CreateInstance<HealthConfig>();
            _created.Add(playerConfig);
            SetPrivate(playerConfig, "_maxHealth", 6);
            SetPrivate(playerConfig, "_invulnerabilityDuration", 1f);
            Health player = CreateBody("Player", Team.Player, Vector3.zero, playerConfig);

            var enemyConfig = ScriptableObject.CreateInstance<EnemyConfig>();
            _created.Add(enemyConfig);
            Health enemy = CreateBody("Enemy", Team.Enemy, new Vector3(0.5f, 0f, 0f), enemyConfig);
            enemy.gameObject.SetActive(false);
            var contact = enemy.gameObject.AddComponent<ContactDamage>();
            SetPrivate(contact, "_config", enemyConfig);
            SetPrivate(contact, "_health", enemy);
            enemy.gameObject.SetActive(true);

            yield return WaitSeconds(0.5f);

            Assert.That(player.Current, Is.EqualTo(5), "one hit, then i-frames");
        }

        private Health CreateTarget(string layer, Team team, Vector3 position, int maxHealth)
        {
            var config = ScriptableObject.CreateInstance<HealthConfig>();
            _created.Add(config);
            SetPrivate(config, "_maxHealth", maxHealth);

            var go = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            _created.Add(go);
            go.layer = LayerMask.NameToLayer(layer);
            go.transform.position = position + Vector3.up;
            var health = go.AddComponent<Health>();
            health.Configure(config, team);
            return health;
        }

        private Health CreateBody(string layer, Team team, Vector3 position, HealthConfig config)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            _created.Add(go);
            go.layer = LayerMask.NameToLayer(layer);
            go.transform.position = position + Vector3.up;
            var body = go.AddComponent<Rigidbody>();
            body.useGravity = false;
            body.constraints = RigidbodyConstraints.FreezeAll;
            var health = go.AddComponent<Health>();
            health.Configure(config, team);
            return health;
        }

        private Projectile CreateProjectile(string layer)
        {
            var go = new GameObject("TestProjectile");
            go.layer = LayerMask.NameToLayer(layer);
            go.transform.position = new Vector3(0f, 0.5f, 0f);
            _created.Add(go);
            return go.AddComponent<Projectile>();
        }

        private static ProjectileStats Stats() => new ProjectileStats(1, 11f, 12f, 0.15f, 0f);

        private static IEnumerator WaitSeconds(float seconds)
        {
            float end = Time.time + seconds;
            while (Time.time < end)
                yield return null;
        }

        private static void SetPrivate(object target, string field, object value)
        {
            System.Type type = target.GetType();
            System.Reflection.FieldInfo info = null;
            while (info == null && type != null)
            {
                info = type.GetField(field, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
                type = type.BaseType;
            }
            Assert.That(info, Is.Not.Null, $"field {field}");
            info.SetValue(target, value);
        }
    }
}
