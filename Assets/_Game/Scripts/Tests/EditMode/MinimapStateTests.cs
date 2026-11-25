using Game.UI;
using NUnit.Framework;

namespace Game.Tests.EditMode
{
    public class MinimapStateTests
    {
        [Test]
        public void Entering_MarksVisited_RevealsNeighbours_AndKeepsTheRestHidden()
        {
            var map = new MinimapState(4);

            map.Enter(0, new[] { 1 });
            Assert.That(map[0], Is.EqualTo(MinimapState.RoomVisibility.Visited));
            Assert.That(map[1], Is.EqualTo(MinimapState.RoomVisibility.Discovered));
            Assert.That(map[2], Is.EqualTo(MinimapState.RoomVisibility.Hidden));

            map.Enter(1, new[] { 0, 2 });
            Assert.That(map.Current, Is.EqualTo(1));
            Assert.That(map[0], Is.EqualTo(MinimapState.RoomVisibility.Visited), "a visited room never goes back to discovered");
            Assert.That(map[2], Is.EqualTo(MinimapState.RoomVisibility.Discovered));
            Assert.That(map[3], Is.EqualTo(MinimapState.RoomVisibility.Hidden));
        }
    }
}
