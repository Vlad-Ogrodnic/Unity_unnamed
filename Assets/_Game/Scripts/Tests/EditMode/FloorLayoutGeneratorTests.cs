using System.Collections.Generic;
using Game.Dungeon;
using NUnit.Framework;
using UnityEngine;

namespace Game.Tests.EditMode
{
    public class FloorLayoutGeneratorTests
    {
        private static readonly FloorGenerationConfig.ShapeWeight[] Shapes =
        {
            new FloorGenerationConfig.ShapeWeight { Size = new Vector2Int(1, 1), Weight = 6f },
            new FloorGenerationConfig.ShapeWeight { Size = new Vector2Int(2, 1), Weight = 2f },
            new FloorGenerationConfig.ShapeWeight { Size = new Vector2Int(2, 2), Weight = 1f },
        };

        private static readonly int[] Seeds = { 1, 7, 42, 1234, 99999, -5 };

        [TestCaseSource(nameof(Seeds))]
        public void Floor_HasOneStartRoom_NoOverlaps_AndEveryRoomReachable(int seed)
        {
            FloorLayout layout = FloorLayoutGenerator.Generate(seed, 8, Shapes);

            Assert.That(layout.Rooms.Count, Is.EqualTo(9));
            Assert.That(layout.Rooms[0].Type, Is.EqualTo(RoomType.Start));
            for (int i = 1; i < layout.Rooms.Count; i++)
                Assert.That(layout.Rooms[i].Type, Is.EqualTo(RoomType.Combat));

            var cells = new HashSet<Vector2Int>();
            foreach (PlacedRoom room in layout.Rooms)
                foreach (Vector2Int cell in room.CoveredCells())
                    Assert.That(cells.Add(cell), Is.True, $"cell {cell} used twice");

            var seen = new HashSet<int> { 0 };
            var queue = new Queue<int>();
            queue.Enqueue(0);
            while (queue.Count > 0)
                foreach (int n in layout.Neighbours(queue.Dequeue()))
                    if (seen.Add(n))
                        queue.Enqueue(n);
            Assert.That(seen.Count, Is.EqualTo(layout.Rooms.Count), "every room reachable from the start");
        }

        [TestCaseSource(nameof(Seeds))]
        public void Connections_JoinNeighbouringCellsOfTheRightRooms(int seed)
        {
            FloorLayout layout = FloorLayoutGenerator.Generate(seed, 8, Shapes);
            foreach (RoomConnection c in layout.Connections)
            {
                Assert.That(layout.RoomAt(c.CellA), Is.EqualTo(c.RoomA));
                Assert.That(layout.RoomAt(c.CellB), Is.EqualTo(c.RoomB));
            }
        }

        [Test]
        public void SameSeed_GivesSameFloor()
        {
            FloorLayout a = FloorLayoutGenerator.Generate(42, 8, Shapes);
            FloorLayout b = FloorLayoutGenerator.Generate(42, 8, Shapes);

            Assert.That(b.Rooms.Count, Is.EqualTo(a.Rooms.Count));
            for (int i = 0; i < a.Rooms.Count; i++)
            {
                Assert.That(b.Rooms[i].Origin, Is.EqualTo(a.Rooms[i].Origin));
                Assert.That(b.Rooms[i].Size, Is.EqualTo(a.Rooms[i].Size));
            }
            Assert.That(b.Connections.Count, Is.EqualTo(a.Connections.Count));
        }
    }
}
