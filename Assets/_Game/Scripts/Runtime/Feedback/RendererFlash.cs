using UnityEngine;

namespace Game.Feedback
{
    /// <summary>Swaps a set of renderers between their own materials and one flash material, without allocating.</summary>
    public sealed class RendererFlash
    {
        private readonly Renderer[] _renderers;
        private readonly Material[][] _original;
        private readonly Material[][] _flash;

        public bool IsFlashing { get; private set; }

        public RendererFlash(Renderer[] renderers, Material flashMaterial)
        {
            _renderers = renderers;
            _original = new Material[renderers.Length][];
            _flash = new Material[renderers.Length][];
            for (int i = 0; i < renderers.Length; i++)
            {
                _original[i] = renderers[i].sharedMaterials;
                _flash[i] = new Material[_original[i].Length];
                for (int m = 0; m < _flash[i].Length; m++)
                    _flash[i][m] = flashMaterial;
            }
        }

        public void Set(bool flashing)
        {
            if (IsFlashing == flashing)
                return;

            IsFlashing = flashing;
            Material[][] materials = flashing ? _flash : _original;
            for (int i = 0; i < _renderers.Length; i++)
            {
                if (_renderers[i] != null)
                    _renderers[i].sharedMaterials = materials[i];
            }
        }
    }
}
