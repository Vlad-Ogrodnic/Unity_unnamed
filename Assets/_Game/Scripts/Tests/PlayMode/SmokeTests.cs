using System.Collections;
using NUnit.Framework;
using UnityEngine.TestTools;

namespace Game.Tests.PlayMode
{
    // Proves the PlayMode test assembly compiles and can run a frame.
    public class SmokeTests
    {
        [UnityTest]
        public IEnumerator PlayModeRunsAFrame()
        {
            yield return null;
            Assert.Pass();
        }
    }
}
