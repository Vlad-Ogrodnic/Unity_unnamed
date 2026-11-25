using Game.Core;
using NUnit.Framework;
using UnityEngine;

namespace Game.Tests.EditMode
{
    public class DamageInfoTests
    {
        [Test]
        public void Direction_IsFlattenedToXZAndNormalized()
        {
            var info = new DamageInfo(1, new Vector3(3f, 5f, 4f), 2f, null);

            Assert.That(info.Direction.y, Is.EqualTo(0f));
            Assert.That(info.Direction.magnitude, Is.EqualTo(1f).Within(1e-5f));
            Assert.That(info.Direction.x, Is.EqualTo(0.6f).Within(1e-5f));
        }

        [Test]
        public void Direction_VerticalOrZero_BecomesZero()
        {
            Assert.That(new DamageInfo(1, Vector3.up, 0f, null).Direction, Is.EqualTo(Vector3.zero));
            Assert.That(new DamageInfo(1, Vector3.zero, 0f, null).Direction, Is.EqualTo(Vector3.zero));
        }
    }
}
