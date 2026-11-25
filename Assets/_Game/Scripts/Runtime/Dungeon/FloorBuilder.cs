using System;
using System.Collections.Generic;
using Game.CameraRig;
using Game.Rooms;
using UnityEngine;

namespace Game.Dungeon
{
    /// <summary>
    /// Generates a floor layout, instantiates a matching template for every room, connects the
    /// doors, registers the rooms with the camera and puts the player in the start room.
    /// </summary>
    public sealed class FloorBuilder : MonoBehaviour
    {
        [SerializeField] private FloorGenerationConfig _config;
        [SerializeField] private RoomCatalog _catalog;
        [SerializeField] private RoomCameraRig _cameraRig;
        [SerializeField] private Transform _player;

        [Tooltip("Seed of the last generated floor (read-only; copy it into the config's Fixed Seed to replay a floor).")]
        [SerializeField] private int _lastSeed;

        private readonly List<Room> _rooms = new List<Room>();

        /// <summary>Raised after a floor is built. Rooms are listed in the same order as the layout's rooms.</summary>
        public event Action<FloorLayout, IReadOnlyList<Room>> FloorBuilt;

        public FloorLayout Layout { get; private set; }
        public IReadOnlyList<Room> Rooms => _rooms;

        private void Start()
        {
            Build(_config.FixedSeed != 0 ? _config.FixedSeed : Environment.TickCount);
        }

        public void Build(int seed)
        {
            _lastSeed = seed;
            var shapes = new List<FloorGenerationConfig.ShapeWeight>();
            foreach (FloorGenerationConfig.ShapeWeight shape in _config.Shapes)
            {
                if (shape.Weight > 0f && _catalog.HasCombatRoom(shape.Size))
                    shapes.Add(shape);
            }

            Layout = FloorLayoutGenerator.Generate(seed, _config.CombatRoomCount, shapes);
            var random = new System.Random(seed);

            foreach (PlacedRoom placed in Layout.Rooms)
            {
                Room template = _catalog.Pick(placed.Type, placed.Size, random);
                Vector3 position = new Vector3(
                    (placed.Origin.x + placed.Size.x * 0.5f) * RoomGrid.CellSize.x,
                    0f,
                    (placed.Origin.y + placed.Size.y * 0.5f) * RoomGrid.CellSize.y);
                Room room = Instantiate(template, position, Quaternion.identity, transform);
                room.name = $"{template.name} #{placed.Index}";
                _rooms.Add(room);
            }

            foreach (RoomConnection connection in Layout.Connections)
            {
                ConnectSide(connection.RoomA, connection.CellA, connection.SideA);
                ConnectSide(connection.RoomB, connection.CellB, connection.SideB);
            }

            foreach (Room room in _rooms)
                _cameraRig.Register(room);

            Room start = _rooms[0];
            if (_player.TryGetComponent(out Rigidbody body))
                body.position = start.transform.position;
            _player.position = start.transform.position;

            FloorBuilt?.Invoke(Layout, _rooms);
        }

        private void ConnectSide(int roomIndex, Vector2Int cell, DoorSide side)
        {
            PlacedRoom placed = Layout.Rooms[roomIndex];
            Door door = _rooms[roomIndex].FindDoor(cell - placed.Origin, side);
            door.Connect();
        }
    }
}
