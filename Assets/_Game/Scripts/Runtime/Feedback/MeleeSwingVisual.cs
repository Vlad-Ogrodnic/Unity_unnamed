using Game.Combat;
using UnityEngine;

namespace Game.Feedback
{
    /// <summary>
    /// Lives inside <c>Visual</c> on an object with a MeshRenderer: shows a flat wedge matching
    /// the melee arc and range for a moment on every swing. The mesh is built from the melee config.
    /// </summary>
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public sealed class MeleeSwingVisual : MonoBehaviour
    {
        private const int Segments = 16;

        [SerializeField] private MeleeAttack _melee;
        [SerializeField] private FeedbackConfig _config;

        private MeshRenderer _renderer;
        private Mesh _mesh;
        private float _hideTime;

        private void Awake()
        {
            _renderer = GetComponent<MeshRenderer>();
            _mesh = BuildWedge(_melee.Config.ArcDegrees, _melee.Config.Range);
            GetComponent<MeshFilter>().sharedMesh = _mesh;
            _renderer.enabled = false;
        }

        private void OnDestroy()
        {
            if (_mesh != null)
                Destroy(_mesh);
        }

        private void OnEnable() => _melee.Swung += OnSwung;

        private void OnDisable()
        {
            _melee.Swung -= OnSwung;
            _renderer.enabled = false;
        }

        private void Update()
        {
            if (_renderer.enabled && Time.unscaledTime >= _hideTime)
                _renderer.enabled = false;
        }

        private void OnSwung(Vector3 direction)
        {
            transform.rotation = Quaternion.LookRotation(direction, Vector3.up);
            _hideTime = Time.unscaledTime + _config.MeleeSwingDuration;
            _renderer.enabled = true;
        }

        /// <summary>A flat fan on the local XZ plane, centred on +Z.</summary>
        private static Mesh BuildWedge(float arcDegrees, float radius)
        {
            var vertices = new Vector3[Segments + 2];
            var triangles = new int[Segments * 3];
            vertices[0] = Vector3.zero;
            float start = -arcDegrees * 0.5f;
            for (int i = 0; i <= Segments; i++)
            {
                float angle = (start + arcDegrees * i / Segments) * Mathf.Deg2Rad;
                vertices[i + 1] = new Vector3(Mathf.Sin(angle), 0f, Mathf.Cos(angle)) * radius;
            }
            for (int i = 0; i < Segments; i++)
            {
                triangles[i * 3] = 0;
                triangles[i * 3 + 1] = i + 1;
                triangles[i * 3 + 2] = i + 2;
            }

            var mesh = new Mesh { name = "MeleeSwingWedge", vertices = vertices, triangles = triangles };
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            return mesh;
        }
    }
}
