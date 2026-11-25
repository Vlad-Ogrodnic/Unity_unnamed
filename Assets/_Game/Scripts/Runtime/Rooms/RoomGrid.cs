using UnityEngine;

namespace Game.Rooms
{
    /// <summary>
    /// Fixed dimensions of the floor grid (1 unit = 1 m). A room covers 1×1, 2×1, 1×2 or 2×2 cells;
    /// each room has its own 1 m walls, so neighbouring rooms touch wall to wall.
    /// </summary>
    public static class RoomGrid
    {
        /// <summary>Outer size of one cell, walls included (X = width, Y = depth along Z).</summary>
        public static readonly Vector2Int CellSize = new Vector2Int(20, 12);

        public const int WallThickness = 1;
        public const float WallHeight = 1.5f;

        /// <summary>Doors are this many tiles wide, centred on each cell's side.</summary>
        public const int DoorWidth = 2;

        /// <summary>Walkable interior of a room covering <paramref name="cells"/>, in tiles.</summary>
        public static Vector2Int InteriorSize(Vector2Int cells) =>
            new Vector2Int(cells.x * CellSize.x - 2 * WallThickness, cells.y * CellSize.y - 2 * WallThickness);

        /// <summary>Outer size of a room covering <paramref name="cells"/>, in m.</summary>
        public static Vector2Int OuterSize(Vector2Int cells) =>
            new Vector2Int(cells.x * CellSize.x, cells.y * CellSize.y);

        /// <summary>
        /// First interior column (north/south doors) or row counted from the south (east/west doors)
        /// of the door on the given cell index along that side.
        /// </summary>
        public static int DoorStart(int cellIndex, int cellLength) =>
            cellIndex * cellLength + cellLength / 2 - DoorWidth / 2 - WallThickness;
    }
}
