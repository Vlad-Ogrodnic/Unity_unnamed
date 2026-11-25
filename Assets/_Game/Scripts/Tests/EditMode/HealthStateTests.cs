using System;
using Game.Core;
using NUnit.Framework;

namespace Game.Tests.EditMode
{
    public class HealthStateTests
    {
        [Test]
        public void StartsAtMax()
        {
            var state = new HealthState(6, 1f);
            Assert.That(state.Current, Is.EqualTo(6));
            Assert.That(state.IsDead, Is.False);
        }

        [Test]
        public void Damage_ReducesHealth()
        {
            var state = new HealthState(6, 0f);

            Assert.That(state.TryDamage(2, 0f, out bool died), Is.True);
            Assert.That(state.Current, Is.EqualTo(4));
            Assert.That(died, Is.False);
        }

        [Test]
        public void InvulnerabilityFrames_BlockRepeatedDamage_UntilTheyEnd()
        {
            var state = new HealthState(6, 1f);
            state.TryDamage(1, 10f, out _);

            Assert.That(state.TryDamage(1, 10.5f, out _), Is.False);
            Assert.That(state.Current, Is.EqualTo(5));
            Assert.That(state.IsInvulnerable(10.99f), Is.True);

            Assert.That(state.TryDamage(1, 11f, out _), Is.True);
            Assert.That(state.Current, Is.EqualTo(4));
        }

        [Test]
        public void NoInvulnerability_AllowsDamageEveryCall()
        {
            var state = new HealthState(3, 0f);
            state.TryDamage(1, 0f, out _);
            Assert.That(state.TryDamage(1, 0f, out _), Is.True);
        }

        [Test]
        public void Died_ReportedOnlyOnce()
        {
            var state = new HealthState(2, 0f);

            state.TryDamage(1, 0f, out bool firstDied);
            state.TryDamage(5, 0f, out bool secondDied);
            bool applied = state.TryDamage(1, 0f, out bool thirdDied);

            Assert.That(firstDied, Is.False);
            Assert.That(secondDied, Is.True);
            Assert.That(applied, Is.False);
            Assert.That(thirdDied, Is.False);
            Assert.That(state.Current, Is.EqualTo(0));
        }

        [Test]
        public void NonPositiveDamage_IsIgnoredAndGivesNoInvulnerability()
        {
            var state = new HealthState(3, 1f);
            Assert.That(state.TryDamage(0, 0f, out _), Is.False);
            Assert.That(state.IsInvulnerable(0f), Is.False);
        }

        [Test]
        public void Heal_ClampsToMax_AndDoesNothingWhenDead()
        {
            var state = new HealthState(6, 0f);
            state.TryDamage(1, 0f, out _);

            Assert.That(state.Heal(2), Is.EqualTo(1));
            Assert.That(state.Current, Is.EqualTo(6));

            state.TryDamage(6, 0f, out _);
            Assert.That(state.Heal(2), Is.EqualTo(0));
            Assert.That(state.IsDead, Is.True);
        }

        [Test]
        public void NonPositiveMax_Throws()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new HealthState(0, 0f));
        }
    }
}
