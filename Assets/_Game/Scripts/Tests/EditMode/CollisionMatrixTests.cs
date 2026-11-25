using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

namespace Game.Tests.EditMode
{
    /// <summary>
    /// Locks the layers and collision matrix from ARCHITECTURE §3, so a stray edit in
    /// Project Settings shows up as a failing test instead of friendly fire in game.
    /// </summary>
    public class CollisionMatrixTests
    {
        private static readonly string[] GameLayers =
        {
            "Ground", "Obstacle", "Player", "Enemy", "PlayerProjectile",
            "EnemyProjectile", "Pickup", "Interactable", "RoomTrigger"
        };

        // Every pair among the game layers that collides. Everything else must be ignored.
        private static readonly (string, string)[] CollidingPairs =
        {
            ("Ground", "Ground"),
            ("Ground", "Player"),
            ("Ground", "Enemy"),
            ("Obstacle", "Player"),
            ("Obstacle", "Enemy"),
            ("Obstacle", "PlayerProjectile"),
            ("Obstacle", "EnemyProjectile"),
            ("Player", "Enemy"),
            ("Player", "EnemyProjectile"),
            ("Player", "Pickup"),
            ("Player", "Interactable"),
            ("Player", "RoomTrigger"),
            ("Enemy", "Enemy"),
            ("Enemy", "PlayerProjectile"),
        };

        [TestCaseSource(nameof(GameLayers))]
        public void LayerExists(string layerName)
        {
            Assert.That(LayerMask.NameToLayer(layerName), Is.GreaterThanOrEqualTo(0), $"Missing layer '{layerName}'");
        }

        [Test]
        public void MatrixMatchesArchitecture()
        {
            var expected = new HashSet<(string, string)>();
            foreach ((string a, string b) in CollidingPairs)
            {
                expected.Add((a, b));
                expected.Add((b, a));
            }

            var mismatches = new List<string>();
            foreach (string a in GameLayers)
            {
                foreach (string b in GameLayers)
                {
                    bool collides = !Physics.GetIgnoreLayerCollision(LayerMask.NameToLayer(a), LayerMask.NameToLayer(b));
                    if (collides != expected.Contains((a, b)))
                        mismatches.Add($"{a} x {b}: expected {(collides ? "ignore" : "collide")}");
                }
            }

            Assert.That(mismatches, Is.Empty, string.Join("\n", mismatches));
        }
    }
}
