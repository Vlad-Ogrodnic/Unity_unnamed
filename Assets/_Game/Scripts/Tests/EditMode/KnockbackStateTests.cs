using Game.Core;
using NUnit.Framework;
using UnityEngine;

namespace Game.Tests.EditMode
{
    public class KnockbackStateTests
    {
        [Test]
        public void Apply_StartsAtFullForce_AlongFlattenedDirection()
        {
            var knockback = new KnockbackState();
            knockback.Apply(new Vector3(1f, 3f, 0f), 6f, 0.2f);

            Assert.That(knockback.IsActive, Is.True);
            Assert.That(Vector3.Distance(knockback.Velocity, new Vector3(6f, 0f, 0f)), Is.LessThan(1e-4f));
        }

        [Test]
        public void Tick_FadesLinearlyThenStops()
        {
            var knockback = new KnockbackState();
            knockback.Apply(Vector3.forward, 6f, 0.2f);

            knockback.Tick(0.1f);
            Assert.That(knockback.Velocity.z, Is.EqualTo(3f).Within(1e-4f));

            knockback.Tick(0.1f);
            Assert.That(knockback.IsActive, Is.False);
            Assert.That(knockback.Velocity, Is.EqualTo(Vector3.zero));
        }

        [TestCase(0f, 0.2f)]
        [TestCase(5f, 0f)]
        public void Apply_WithoutForceOrDuration_DoesNothing(float force, float duration)
        {
            var knockback = new KnockbackState();
            knockback.Apply(Vector3.forward, force, duration);
            Assert.That(knockback.IsActive, Is.False);
        }

        [Test]
        public void Apply_WithoutDirection_DoesNothing()
        {
            var knockback = new KnockbackState();
            knockback.Apply(Vector3.up, 5f, 0.2f);
            Assert.That(knockback.IsActive, Is.False);
        }
    }
}
