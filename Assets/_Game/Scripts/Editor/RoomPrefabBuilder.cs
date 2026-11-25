using System.Collections.Generic;
using System.IO;
using Game.Dungeon;
using Game.Rooms;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.Editor
{
    /// <summary>
    /// Builds Prefabs/Rooms/Room_*.prefab from the text layouts in Data/RoomLayouts
    /// (format in Data/RoomLayouts/README.md). Prefabs are overwritten on every rebuild.
    /// </summary>
    public static class RoomPrefabBuilder
    {
        private const string LayoutFolder = "Assets/_Game/Data/RoomLayouts";
        private const string PrefabFolder = "Assets/_Game/Prefabs/Rooms";
        private const string RoomConfigPath = "Assets/_Game/Data/RoomConfig.asset";
        private const string CatalogPath = "Assets/_Game/Data/RoomCatalog.asset";
        private const string StartRoomPrefix = "Room_Start";
        private const string MaterialFolder = "Assets/_Game/Materials";

        private static readonly Dictionary<RoomTile, string> EnemyPrefabs = new Dictionary<RoomTile, string>
        {
            { RoomTile.Chaser, "Assets/_Game/Prefabs/Enemies/Enemy_Chaser.prefab" },
            { RoomTile.Shooter, "Assets/_Game/Prefabs/Enemies/Enemy_Shooter.prefab" },
            { RoomTile.Charger, "Assets/_Game/Prefabs/Enemies/Enemy_Charger.prefab" },
        };

        [MenuItem("Game/Rooms/Rebuild Room Prefabs")]
        public static void RebuildAllMenu()
        {
            int built = RebuildAll(out List<string> errors);
            if (errors.Count > 0)
                EditorUtility.DisplayDialog("Rooms", $"Built {built} rooms.\n\nProblems:\n" + string.Join("\n", errors), "OK");
        }

        /// <summary>Rebuilds every valid layout. Returns how many prefabs were written; problems go to <paramref name="errors"/>.</summary>
        public static int RebuildAll(out List<string> errors)
        {
            errors = new List<string>();
            var layouts = new List<RoomLayout>();
            foreach (string path in Directory.GetFiles(LayoutFolder, "Room_*.txt"))
            {
                string name = Path.GetFileNameWithoutExtension(path);
                try
                {
                    RoomLayout layout = RoomLayout.Parse(name, File.ReadAllText(path));
                    List<string> problems = RoomLayoutValidator.Validate(layout);
                    if (problems.Count > 0)
                        errors.AddRange(problems);
                    else
                        layouts.Add(layout);
                }
                catch (System.FormatException e)
                {
                    errors.Add(e.Message);
                }
            }

            // Build in a throwaway scene so nothing touches the scene the user has open.
            Scene previous = SceneManager.GetActiveScene();
            Scene temp = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Additive);
            SceneManager.SetActiveScene(temp);
            var startRooms = new List<Object>();
            var combatRooms = new List<Object>();
            try
            {
                var assets = new BuildAssets();
                foreach (RoomLayout layout in layouts)
                {
                    GameObject room = Build(layout, assets);
                    GameObject prefab = PrefabUtility.SaveAsPrefabAsset(room, $"{PrefabFolder}/{layout.Name}.prefab");
                    Object.DestroyImmediate(room);
                    (layout.Name.StartsWith(StartRoomPrefix) ? startRooms : combatRooms).Add(prefab.GetComponent<Room>());
                }
            }
            finally
            {
                if (previous.IsValid())
                    SceneManager.SetActiveScene(previous);
                EditorSceneManager.CloseScene(temp, true);
            }

            UpdateCatalog(startRooms, combatRooms);
            AssetDatabase.SaveAssets();
            foreach (string error in errors)
                Debug.LogError(error);
            return layouts.Count;
        }

        /// <summary>Lists the built rooms in Data/RoomCatalog.asset: names starting with Room_Start are start rooms.</summary>
        private static void UpdateCatalog(List<Object> startRooms, List<Object> combatRooms)
        {
            var catalog = AssetDatabase.LoadAssetAtPath<RoomCatalog>(CatalogPath);
            if (catalog == null)
            {
                catalog = ScriptableObject.CreateInstance<RoomCatalog>();
                AssetDatabase.CreateAsset(catalog, CatalogPath);
            }
            var so = new SerializedObject(catalog);
            SetArray(so.FindProperty("_startRooms"), startRooms);
            SetArray(so.FindProperty("_combatRooms"), combatRooms);
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void SetArray(SerializedProperty property, List<Object> values)
        {
            property.arraySize = values.Count;
            for (int i = 0; i < values.Count; i++)
                property.GetArrayElementAtIndex(i).objectReferenceValue = values[i];
        }

        private sealed class BuildAssets
        {
            public readonly RoomConfig Config = AssetDatabase.LoadAssetAtPath<RoomConfig>(RoomConfigPath);
            public readonly Material Ground = Load("M_Ground");
            public readonly Material Wall = Load("M_Wall");
            public readonly Material Obstacle = Load("M_Obstacle");
            public readonly Material DoorBars = Load("M_DoorBars");
            public readonly Dictionary<RoomTile, GameObject> Enemies = new Dictionary<RoomTile, GameObject>();

            public BuildAssets()
            {
                foreach (KeyValuePair<RoomTile, string> pair in EnemyPrefabs)
                    Enemies[pair.Key] = AssetDatabase.LoadAssetAtPath<GameObject>(pair.Value);
            }

            private static Material Load(string name) => AssetDatabase.LoadAssetAtPath<Material>($"{MaterialFolder}/{name}.mat");
        }

        private static GameObject Build(RoomLayout layout, BuildAssets assets)
        {
            Vector2Int outer = RoomGrid.OuterSize(layout.Cells);
            Vector2Int interior = layout.Size;
            int wall = RoomGrid.WallThickness;
            float h = RoomGrid.WallHeight;
            float halfW = outer.x * 0.5f, halfD = outer.y * 0.5f;
            int obstacleLayer = LayerMask.NameToLayer("Obstacle");

            var root = new GameObject(layout.Name) { layer = LayerMask.NameToLayer("RoomTrigger") };
            var playArea = root.AddComponent<BoxCollider>();
            playArea.isTrigger = true;
            playArea.center = new Vector3(0f, 1f, 0f);
            playArea.size = new Vector3(interior.x, 2f, interior.y);
            var room = root.AddComponent<Room>();

            BoxCollider floor = Box("Floor", root.transform, new Vector3(0f, -0.5f, 0f), new Vector3(outer.x, 1f, outer.y), assets.Ground, LayerMask.NameToLayer("Ground")).GetComponent<BoxCollider>();

            // Walls, with a gap for every door slot.
            var walls = new GameObject("Walls").transform;
            walls.SetParent(root.transform, false);
            var doorsRoot = new GameObject("Doors").transform;
            doorsRoot.SetParent(root.transform, false);
            var gaps = new Dictionary<DoorSide, List<int>>
            {
                { DoorSide.North, new List<int>() }, { DoorSide.South, new List<int>() },
                { DoorSide.East, new List<int>() }, { DoorSide.West, new List<int>() },
            };
            var doors = new List<Door>();
            foreach (DoorSlot slot in DoorSlot.All(layout.Cells))
            {
                bool horizontal = slot.Side == DoorSide.North || slot.Side == DoorSide.South;
                int start = horizontal
                    ? RoomGrid.DoorStart(slot.Cell.x, RoomGrid.CellSize.x)
                    : RoomGrid.DoorStart(slot.Cell.y, RoomGrid.CellSize.y);
                gaps[slot.Side].Add(start);

                float along = (horizontal ? -halfW : -halfD) + wall + start + RoomGrid.DoorWidth * 0.5f;
                Vector3 centre = DoorCentre(slot.Side, along, halfW, halfD, wall, h);
                Vector3 size = horizontal ? new Vector3(RoomGrid.DoorWidth, h, wall) : new Vector3(wall, h, RoomGrid.DoorWidth);
                doors.Add(BuildDoor(slot, doorsRoot, centre, size, assets, obstacleLayer));
            }

            WallSide(walls, DoorSide.North, gaps[DoorSide.North], outer.x, -halfW, new Vector3(0f, h / 2f, halfD - wall / 2f), assets.Wall, obstacleLayer, true);
            WallSide(walls, DoorSide.South, gaps[DoorSide.South], outer.x, -halfW, new Vector3(0f, h / 2f, -halfD + wall / 2f), assets.Wall, obstacleLayer, true);
            WallSide(walls, DoorSide.East, gaps[DoorSide.East], outer.y, -halfD, new Vector3(halfW - wall / 2f, h / 2f, 0f), assets.Wall, obstacleLayer, false);
            WallSide(walls, DoorSide.West, gaps[DoorSide.West], outer.y, -halfD, new Vector3(-halfW + wall / 2f, h / 2f, 0f), assets.Wall, obstacleLayer, false);

            // Obstacles: merge horizontal runs of '#' into single boxes.
            var obstacles = new GameObject("Obstacles").transform;
            obstacles.SetParent(root.transform, false);
            for (int z = 0; z < interior.y; z++)
            {
                for (int x = 0; x < interior.x; x++)
                {
                    if (layout[x, z] != RoomTile.Obstacle)
                        continue;
                    int end = x;
                    while (end + 1 < interior.x && layout[end + 1, z] == RoomTile.Obstacle)
                        end++;
                    int length = end - x + 1;
                    Vector3 pos = TileCentre(x, z, halfW, halfD, wall) + new Vector3((length - 1) * 0.5f, 0.5f, 0f);
                    Box($"Rock_{x}_{z}", obstacles, pos, new Vector3(length, 1f, 1f), assets.Obstacle, obstacleLayer).isStatic = true;
                    x = end;
                }
            }

            // Enemies start inactive; the room wakes them up.
            var enemies = new GameObject("Enemies").transform;
            enemies.SetParent(root.transform, false);
            for (int z = 0; z < interior.y; z++)
            {
                for (int x = 0; x < interior.x; x++)
                {
                    RoomTile tile = layout[x, z];
                    if (!RoomLayoutValidator.IsEnemy(tile))
                        continue;
                    var enemy = (GameObject)PrefabUtility.InstantiatePrefab(assets.Enemies[tile]);
                    enemy.transform.SetParent(enemies, false);
                    enemy.transform.localPosition = TileCentre(x, z, halfW, halfD, wall);
                    enemy.SetActive(false);
                }
            }

            var drop = new GameObject("DropPoint").transform;
            drop.SetParent(root.transform, false);
            drop.localPosition = NearestFloorToCentre(layout, halfW, halfD, wall);

            Set(room, "_config", assets.Config);
            Set(room, "_cells", layout.Cells);
            Set(room, "_floor", floor);
            Set(room, "_enemiesRoot", enemies);
            Set(room, "_dropPoint", drop);
            var so = new SerializedObject(room);
            SerializedProperty doorsProp = so.FindProperty("_doors");
            doorsProp.arraySize = doors.Count;
            for (int i = 0; i < doors.Count; i++)
                doorsProp.GetArrayElementAtIndex(i).objectReferenceValue = doors[i];
            so.ApplyModifiedPropertiesWithoutUndo();
            return root;
        }

        private static Door BuildDoor(DoorSlot slot, Transform parent, Vector3 centre, Vector3 size, BuildAssets assets, int layer)
        {
            var go = new GameObject($"Door_{slot.Side}_{slot.Cell.x}_{slot.Cell.y}") { layer = layer };
            go.transform.SetParent(parent, false);
            go.transform.localPosition = centre;
            var blocker = go.AddComponent<BoxCollider>();
            blocker.size = size;
            var door = go.AddComponent<Door>();
            Set(door, "_side", slot.Side);
            Set(door, "_cell", slot.Cell);
            Set(door, "_blocker", blocker);

            var visual = new GameObject("Visual") { layer = layer };
            visual.transform.SetParent(go.transform, false);
            GameObject wallPiece = Box("WallPiece", visual.transform, Vector3.zero, size, assets.Wall, layer, false);
            Vector3 barsSize = Vector3.Scale(size, new Vector3(slot.Side == DoorSide.North || slot.Side == DoorSide.South ? 1f : 0.4f, 1f, slot.Side == DoorSide.North || slot.Side == DoorSide.South ? 0.4f : 1f));
            GameObject bars = Box("Bars", visual.transform, Vector3.zero, barsSize, assets.DoorBars, layer, false);
            var doorVisual = visual.AddComponent<DoorVisual>();
            Set(doorVisual, "_door", door);
            Set(doorVisual, "_wallPiece", wallPiece);
            Set(doorVisual, "_bars", bars);
            bars.SetActive(false);
            return door;
        }

        private static Vector3 DoorCentre(DoorSide side, float along, float halfW, float halfD, int wall, float h)
        {
            switch (side)
            {
                case DoorSide.North: return new Vector3(along, h / 2f, halfD - wall / 2f);
                case DoorSide.South: return new Vector3(along, h / 2f, -halfD + wall / 2f);
                case DoorSide.East: return new Vector3(halfW - wall / 2f, h / 2f, along);
                default: return new Vector3(-halfW + wall / 2f, h / 2f, along);
            }
        }

        /// <summary>One wall side as segments between the door gaps. Gap starts are interior tile indices.</summary>
        private static void WallSide(Transform parent, DoorSide side, List<int> gapStarts, int outerLength, float outerMin, Vector3 centre, Material material, int layer, bool horizontal)
        {
            int wall = RoomGrid.WallThickness;
            // Along-axis range covered by this side: N/S span the full width, E/W only between the N/S walls.
            float from = horizontal ? outerMin : outerMin + wall;
            float to = horizontal ? -outerMin : -outerMin - wall;
            gapStarts.Sort();
            var cuts = new List<(float, float)>();
            foreach (int start in gapStarts)
            {
                float a = outerMin + wall + start;
                cuts.Add((a, a + RoomGrid.DoorWidth));
            }

            float cursor = from;
            int index = 0;
            foreach ((float gapFrom, float gapTo) in cuts)
            {
                Segment(parent, side, index++, cursor, gapFrom, centre, material, layer, horizontal);
                cursor = gapTo;
            }
            Segment(parent, side, index, cursor, to, centre, material, layer, horizontal);
        }

        private static void Segment(Transform parent, DoorSide side, int index, float a, float b, Vector3 centre, Material material, int layer, bool horizontal)
        {
            if (b - a < 0.01f)
                return;
            float mid = (a + b) * 0.5f;
            float length = b - a;
            int wall = RoomGrid.WallThickness;
            Vector3 pos = horizontal ? new Vector3(mid, centre.y, centre.z) : new Vector3(centre.x, centre.y, mid);
            Vector3 size = horizontal ? new Vector3(length, RoomGrid.WallHeight, wall) : new Vector3(wall, RoomGrid.WallHeight, length);
            Box($"Wall_{side}_{index}", parent, pos, size, material, layer).isStatic = true;
        }

        private static Vector3 TileCentre(int x, int z, float halfW, float halfD, int wall) =>
            new Vector3(-halfW + wall + x + 0.5f, 0f, -halfD + wall + z + 0.5f);

        private static Vector3 NearestFloorToCentre(RoomLayout layout, float halfW, float halfD, int wall)
        {
            Vector2 centre = new Vector2(layout.Size.x, layout.Size.y) * 0.5f;
            Vector2Int best = Vector2Int.zero;
            float bestDistance = float.MaxValue;
            for (int x = 0; x < layout.Size.x; x++)
            {
                for (int z = 0; z < layout.Size.y; z++)
                {
                    if (layout[x, z] == RoomTile.Obstacle || layout[x, z] == RoomTile.Destructible)
                        continue;
                    float d = (new Vector2(x + 0.5f, z + 0.5f) - centre).sqrMagnitude;
                    if (d < bestDistance)
                    {
                        bestDistance = d;
                        best = new Vector2Int(x, z);
                    }
                }
            }
            return TileCentre(best.x, best.y, halfW, halfD, wall);
        }

        private static GameObject Box(string name, Transform parent, Vector3 pos, Vector3 size, Material material, int layer, bool collider = true)
        {
            GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.name = name;
            go.layer = layer;
            if (!collider)
                Object.DestroyImmediate(go.GetComponent<Collider>());
            go.transform.SetParent(parent, false);
            go.transform.localPosition = pos;
            go.transform.localScale = size;
            go.GetComponent<MeshRenderer>().sharedMaterial = material;
            return go;
        }

        private static void Set(Object target, string field, object value)
        {
            var so = new SerializedObject(target);
            SerializedProperty p = so.FindProperty(field);
            switch (value)
            {
                case Object o: p.objectReferenceValue = o; break;
                case Vector2Int v: p.vector2IntValue = v; break;
                case DoorSide side: p.enumValueIndex = (int)side; break;
                default: throw new System.ArgumentException($"Unsupported value for {field}");
            }
            so.ApplyModifiedPropertiesWithoutUndo();
        }
    }
}
