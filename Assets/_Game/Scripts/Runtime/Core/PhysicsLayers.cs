using UnityEngine;

namespace Game.Core
{
    public static class PhysicsLayers
    {
        /// <summary>Layers that collide with <paramref name="layer"/> according to the collision matrix (Project Settings).</summary>
        public static int CollisionMaskFor(int layer)
        {
            int mask = 0;
            for (int other = 0; other < 32; other++)
            {
                if (!Physics.GetIgnoreLayerCollision(layer, other))
                    mask |= 1 << other;
            }
            return mask;
        }
    }
}
