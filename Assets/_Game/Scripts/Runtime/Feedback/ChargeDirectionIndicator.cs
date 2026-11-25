using Game.Enemies;
using UnityEngine;

namespace Game.Feedback
{
    /// <summary>
    /// Lives inside a charger's <c>Visual</c> on an object with a MeshRenderer: shows a flat arrow on
    /// the floor, pointing where the charge will go, for as long as the charger telegraphs.
    /// </summary>
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public sealed class ChargeDirectionIndicator : MonoBehaviour
    {
        [SerializeField] private ChargerBehaviour _charger;

        [Header("Arrow shape (visual only)")]
        [SerializeField, Min(0.1f)] private float _startOffset = 0.6f;
        [SerializeField, Min(0.1f)] private float _length = 2.2f;
        [SerializeField, Min(0.05f)] private float _shaftWidth = 0.25f;
        [SerializeField, Min(0.05f)] private float _headWidth = 0.7f;
        [SerializeField, Min(0.05f)] private float _headLength = 0.6f;

        private MeshRenderer _renderer;
        private Mesh _mesh;

        private void Awake()
        {
            _renderer = GetComponent<MeshRenderer>();
            _mesh = BuildArrow();
            GetComponent<MeshFilter>().sharedMesh = _mesh;
            _renderer.enabled = false;
        }

        private void OnDestroy()
        {
            if (_mesh != null)
                Destroy(_mesh);
        }

        private void OnEnable()
        {
            _charger.TelegraphStarted += OnStarted;
            _charger.TelegraphEnded += OnEnded;
        }

        private void OnDisable()
        {
            _charger.TelegraphStarted -= OnStarted;
            _charger.TelegraphEnded -= OnEnded;
            _renderer.enabled = false;
        }

        private void OnStarted(Vector3 direction)
        {
            transform.rotation = Quaternion.LookRotation(direction, Vector3.up);
            _renderer.enabled = true;
        }

        private void OnEnded() => _renderer.enabled = false;

        /// <summary>A flat arrow on the local XZ plane, pointing along +Z.</summary>
        private Mesh BuildArrow()
        {
            float shaftEnd = _startOffset + _length - _headLength;
            float tip = _startOffset + _length;
            float s = _shaftWidth * 0.5f;
            float h = _headWidth * 0.5f;
            var vertices = new[]
            {
                new Vector3(-s, 0f, _startOffset), new Vector3(s, 0f, _startOffset),
                new Vector3(-s, 0f, shaftEnd), new Vector3(s, 0f, shaftEnd),
                new Vector3(-h, 0f, shaftEnd), new Vector3(h, 0f, shaftEnd),
                new Vector3(0f, 0f, tip),
            };
            var triangles = new[] { 0, 2, 1, 1, 2, 3, 4, 6, 5 };
            var mesh = new Mesh { name = "ChargeArrow", vertices = vertices, triangles = triangles };
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            return mesh;
        }
    }
}
