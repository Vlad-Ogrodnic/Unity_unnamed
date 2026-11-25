using Game.Core;
using NUnit.Framework;
using UnityEngine;

namespace Game.Tests.EditMode
{
    public class ArcTests
    {
        [TestCase(0f, true)]
        [TestCase(59f, true)]
        [TestCase(-59f, true)]
        [TestCase(61f, false)]
        [TestCase(180f, false)]
        public void IsWithinArc_120Degrees(float angleFromForward, bool expected)
        {
            Vector3 toTarget = Quaternion.Euler(0f, angleFromForward, 0f) * Vector3.forward;
            Assert.That(PlanarMath.IsWithinArc(Vector3.forward, toTarget, 120f), Is.EqualTo(expected));
        }

        [Test]
        public void IsWithinArc_IgnoresHeight_AndCountsZeroOffsetAsInside()
        {
            Assert.That(PlanarMath.IsWithinArc(Vector3.forward, new Vector3(0f, 5f, 1f), 10f), Is.True);
            Assert.That(PlanarMath.IsWithinArc(Vector3.forward, Vector3.up, 10f), Is.True);
        }
    }
}
