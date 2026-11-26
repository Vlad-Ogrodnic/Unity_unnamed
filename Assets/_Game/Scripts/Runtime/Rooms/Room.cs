using System;
using System.Collections.Generic;
using Game.Core;
using Game.Enemies;
using UnityEngine;

namespace Game.Rooms
{
    /// <summary>
    /// One room of a floor. Unvisited → (player enters) → Active: doors close, enemies wake up and get
    /// the player as target → (last enemy dies) → Cleared: doors open, maybe a drop. Rooms without
    /// enemies are cleared on entry. The room's own collider (a trigger over the walkable floor,
    /// on the RoomTrigger layer) detects the player.
    /// </summary>
    [RequireComponent(typeof(BoxCollider))]
    public sealed class Room : MonoBehaviour
    {
        public enum RoomState
        {
            Unvisited,
            Active,
            Cleared
        }

        [SerializeField] private RoomConfig _config;
        [SerializeField] private Vector2Int _cells = Vector2Int.one;

        [Tooltip("Floor collider covering the room including its walls (for camera framing).")]
        [SerializeField] private BoxCollider _floor;

        [SerializeField] private Door[] _doors = Array.Empty<Door>();

        [Tooltip("Parent of the room's enemies; they start inactive and wake up when the player enters.")]
        [SerializeField] private Transform _enemiesRoot;

        [SerializeField] private Transform _dropPoint;

        private readonly List<Health> _enemies = new List<Health>();
        private BoxCollider _playArea;
        private int _aliveEnemies;

        /// <summary>The player walked into this room (every time, not only the first).</summary>
        public event Action<Room, Transform> PlayerEntered;

        public event Action<Room> Cleared;

        public RoomState State { get; private set; } = RoomState.Unvisited;
        public Vector2Int Cells => _cells;
        public IReadOnlyList<Door> Doors => _doors;

        /// <summary>Walkable floor of the room, flattened to the floor plane.</summary>
        public Bounds PlayArea => Flatten(_playArea.bounds, _playArea.bounds.min.y);

        /// <summary>The room including its walls, flattened to the floor plane.</summary>
        public Bounds RoomArea => Flatten(_floor.bounds, _floor.bounds.max.y);

        private void Awake()
        {
            _playArea = GetComponent<BoxCollider>();
            if (_enemiesRoot == null)
                return;

            foreach (Transform child in _enemiesRoot)
            {
                if (child.TryGetComponent(out Health health))
                {
                    _enemies.Add(health);
                    child.gameObject.SetActive(false);
                }
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            Health health = other.GetComponentInParent<Health>();
            if (health == null || health.Team != Team.Player)
                return;

            PlayerEntered?.Invoke(this, health.transform);
            if (State == RoomState.Unvisited)
                Activate(health.transform);
        }

        private void Activate(Transform player)
        {
            if (_enemies.Count == 0)
            {
                State = RoomState.Cleared;
                Cleared?.Invoke(this);
                return;
            }

            State = RoomState.Active;
            foreach (Door door in _doors)
                door.Close();

            _aliveEnemies = _enemies.Count;
            foreach (Health enemy in _enemies)
            {
                enemy.Died += OnEnemyDied;
                enemy.gameObject.SetActive(true);
                foreach (IEnemyBehaviour behaviour in enemy.GetComponents<IEnemyBehaviour>())
                    behaviour.SetTarget(player);
            }
        }

        private void OnEnemyDied()
        {
            _aliveEnemies--;
            if (_aliveEnemies > 0)
                return;

            foreach (Health enemy in _enemies)
                enemy.Died -= OnEnemyDied;

            State = RoomState.Cleared;
            foreach (Door door in _doors)
                door.Open();

            if (_config != null && _config.PotionPickupPrefab != null && UnityEngine.Random.value < _config.PotionDropChance)
                Instantiate(_config.PotionPickupPrefab, _dropPoint.position, Quaternion.identity, transform);

            Cleared?.Invoke(this);
        }

        /// <summary>The door on a given cell side, or null.</summary>
        public Door FindDoor(Vector2Int cell, DoorSide side)
        {
            foreach (Door door in _doors)
            {
                if (door.Cell == cell && door.Side == side)
                    return door;
            }
            return null;
        }

        private static Bounds Flatten(Bounds b, float y) =>
            new Bounds(new Vector3(b.center.x, y, b.center.z), new Vector3(b.size.x, 0f, b.size.z));
    }
}
