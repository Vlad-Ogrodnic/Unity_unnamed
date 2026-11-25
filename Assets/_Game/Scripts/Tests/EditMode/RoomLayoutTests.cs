using System.IO;
using Game.Rooms;
using NUnit.Framework;
using UnityEngine;

namespace Game.Tests.EditMode
{
    public class RoomLayoutTests
    {
        private static string[] LayoutFiles() =>
            Directory.GetFiles(Path.Combine(Application.dataPath, "_Game/Data/RoomLayouts"), "Room_*.txt");

        [TestCaseSource(nameof(LayoutFiles))]
        public void EveryLayout_FollowsTheRoomRules(string path)
        {
            RoomLayout layout = RoomLayout.Parse(Path.GetFileNameWithoutExtension(path), File.ReadAllText(path));
            Assert.That(RoomLayoutValidator.Validate(layout), Is.Empty);
        }

        [Test]
        public void Validator_CatchesBlockedDoorUnreachableTilesAndEnemiesAtTheDoor()
        {
            // 1x1: '#' in front of the west door, a walled-off pocket in the north-east, an enemy at the south door.
            string text = "size: 1x1\n" +
                          "..............#...\n" +
                          "..............#...\n" +
                          "..............####\n" +
                          "..................\n" +
                          "#.................\n" +
                          "..................\n" +
                          "..................\n" +
                          "..................\n" +
                          "..................\n" +
                          "........C.........\n";
            var errors = RoomLayoutValidator.Validate(RoomLayout.Parse("Bad", text));

            Assert.That(errors.Exists(e => e.Contains("door approach")), Is.True, "blocked door");
            Assert.That(errors.Exists(e => e.Contains("can't be reached")), Is.True, "unreachable pocket");
            Assert.That(errors.Exists(e => e.Contains("closer than")), Is.True, "enemy at the door");
        }
    }
}
