using System.Collections.Generic;
using Game.Dungeon;
using Game.Rooms;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    /// <summary>
    /// Draws the floor as rectangles inside this RectTransform (top-right of the HUD), scaled to fit.
    /// Shows only what <see cref="MinimapState"/> allows.
    /// </summary>
    [RequireComponent(typeof(RectTransform))]
    public sealed class MinimapView : MonoBehaviour
    {
        [SerializeField] private FloorBuilder _floor;

        [Header("Look (visual only)")]
        [SerializeField] private Color _visited = new Color(0.75f, 0.75f, 0.78f, 0.9f);
        [SerializeField] private Color _current = Color.white;
        [SerializeField] private Color _discovered = new Color(0.35f, 0.35f, 0.4f, 0.8f);
        [SerializeField, Min(0f)] private float _gapPixels = 2f;

        private readonly List<Image> _images = new List<Image>();
        private readonly List<Room> _rooms = new List<Room>();
        private FloorLayout _layout;
        private MinimapState _state;

        private void OnEnable() => _floor.FloorBuilt += OnFloorBuilt;

        private void OnDisable()
        {
            _floor.FloorBuilt -= OnFloorBuilt;
            foreach (Room room in _rooms)
            {
                if (room != null)
                    room.PlayerEntered -= OnPlayerEntered;
            }
        }

        private void OnFloorBuilt(FloorLayout layout, IReadOnlyList<Room> rooms)
        {
            _layout = layout;
            _state = new MinimapState(layout.Rooms.Count);
            _rooms.Clear();
            _rooms.AddRange(rooms);
            foreach (Room room in _rooms)
                room.PlayerEntered += OnPlayerEntered;
            CreateImages();
            Refresh();
        }

        private void OnPlayerEntered(Room room, Transform _)
        {
            int index = _rooms.IndexOf(room);
            if (index < 0)
                return;
            _state.Enter(index, _layout.Neighbours(index));
            Refresh();
        }

        private void CreateImages()
        {
            foreach (Image image in _images)
                Destroy(image.gameObject);
            _images.Clear();

            // Fit the whole floor's cell bounds into this rect, keeping the cells' proportions.
            Vector2Int min = new Vector2Int(int.MaxValue, int.MaxValue), max = new Vector2Int(int.MinValue, int.MinValue);
            foreach (PlacedRoom room in _layout.Rooms)
            {
                min = Vector2Int.Min(min, room.Origin);
                max = Vector2Int.Max(max, room.Origin + room.Size);
            }
            Rect area = ((RectTransform)transform).rect;
            Vector2 cellShape = RoomGrid.CellSize;
            Vector2 floorSize = Vector2.Scale(max - min, cellShape);
            float scale = Mathf.Min(area.width / floorSize.x, area.height / floorSize.y);
            Vector2 offset = (area.size - floorSize * scale) * 0.5f;

            foreach (PlacedRoom room in _layout.Rooms)
            {
                var go = new GameObject($"MapRoom {room.Index}", typeof(RectTransform), typeof(Image));
                var rect = (RectTransform)go.transform;
                rect.SetParent(transform, false);
                rect.anchorMin = rect.anchorMax = Vector2.zero;
                rect.pivot = Vector2.zero;
                rect.anchoredPosition = offset + Vector2.Scale(room.Origin - min, cellShape) * scale + Vector2.one * (_gapPixels * 0.5f);
                rect.sizeDelta = Vector2.Scale(room.Size, cellShape) * scale - Vector2.one * _gapPixels;
                var image = go.GetComponent<Image>();
                image.raycastTarget = false;
                _images.Add(image);
            }
        }

        private void Refresh()
        {
            for (int i = 0; i < _images.Count; i++)
            {
                MinimapState.RoomVisibility visibility = _state[i];
                _images[i].enabled = visibility != MinimapState.RoomVisibility.Hidden;
                _images[i].color = i == _state.Current ? _current
                    : visibility == MinimapState.RoomVisibility.Visited ? _visited : _discovered;
            }
        }
    }
}
