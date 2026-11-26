using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Rooms
{
    public enum RoomTile
    {
        Floor,
        Obstacle,
        Destructible,
        Chaser,
        Shooter,
        Charger
    }

    /// <summary>
    /// A room described as text. First line: <c>size: WxH</c> in cells (e.g. <c>size: 2x1</c>),
    /// then one line per row of interior tiles, north row first. Lines starting with <c>//</c> are comments.
    /// Legend: <c>.</c> floor, <c>#</c> obstacle, <c>X</c> destructible obstacle (reserved, not built yet),
    /// <c>C</c> Chaser, <c>S</c> Shooter, <c>R</c> Charger. See Data/RoomLayouts/README.md.
    /// </summary>
    public sealed class RoomLayout
    {
        private readonly RoomTile[,] _tiles; // [x, z], z = 0 is the south row

        public string Name { get; }
        public Vector2Int Cells { get; }
        public Vector2Int Size => RoomGrid.InteriorSize(Cells);

        private RoomLayout(string name, Vector2Int cells, RoomTile[,] tiles)
        {
            Name = name;
            Cells = cells;
            _tiles = tiles;
        }

        /// <summary>Tile at interior column <paramref name="x"/> and row <paramref name="z"/> (0 = south).</summary>
        public RoomTile this[int x, int z] => _tiles[x, z];

        public bool InBounds(int x, int z) => x >= 0 && z >= 0 && x < Size.x && z < Size.y;

        public static RoomLayout Parse(string name, string text)
        {
            var rows = new List<string>();
            Vector2Int? cells = null;
            foreach (string raw in text.Split('\n'))
            {
                string line = raw.TrimEnd('\r', ' ', '\t');
                if (line.Length == 0 || line.StartsWith("//", StringComparison.Ordinal))
                    continue;

                if (cells == null)
                {
                    cells = ParseSize(name, line);
                    continue;
                }
                rows.Add(line);
            }

            if (cells == null)
                throw new FormatException($"{name}: missing 'size: WxH' line.");

            Vector2Int size = RoomGrid.InteriorSize(cells.Value);
            if (rows.Count != size.y)
                throw new FormatException($"{name}: expected {size.y} rows for size {cells.Value.x}x{cells.Value.y}, found {rows.Count}.");

            var tiles = new RoomTile[size.x, size.y];
            for (int r = 0; r < rows.Count; r++)
            {
                string row = rows[r];
                if (row.Length != size.x)
                    throw new FormatException($"{name}: row {r + 1} has {row.Length} tiles, expected {size.x}.");

                int z = size.y - 1 - r;
                for (int x = 0; x < size.x; x++)
                    tiles[x, z] = ParseTile(name, row[x], r + 1, x + 1);
            }

            return new RoomLayout(name, cells.Value, tiles);
        }

        private static Vector2Int ParseSize(string name, string line)
        {
            const string prefix = "size:";
            if (!line.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                throw new FormatException($"{name}: first line must be 'size: WxH', got '{line}'.");

            string[] parts = line.Substring(prefix.Length).Trim().Split('x', 'X');
            if (parts.Length != 2 || !int.TryParse(parts[0], out int w) || !int.TryParse(parts[1], out int h)
                || w < 1 || h < 1 || w > 2 || h > 2)
                throw new FormatException($"{name}: size must be 1x1, 2x1, 1x2 or 2x2, got '{line}'.");

            return new Vector2Int(w, h);
        }

        private static RoomTile ParseTile(string name, char c, int row, int column)
        {
            switch (c)
            {
                case '.': return RoomTile.Floor;
                case '#': return RoomTile.Obstacle;
                case 'X': return RoomTile.Destructible;
                case 'C': return RoomTile.Chaser;
                case 'S': return RoomTile.Shooter;
                case 'R': return RoomTile.Charger;
                default:
                    throw new FormatException($"{name}: unknown tile '{c}' at row {row}, column {column}.");
            }
        }
    }
}
