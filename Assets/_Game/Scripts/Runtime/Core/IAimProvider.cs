using UnityEngine;

namespace Game.Core
{
    /// <summary>
    /// Where an attacker is aiming. Attacks depend only on this, so the aiming method
    /// (mouse, enemy AI) can change without touching weapon code.
    /// </summary>
    public interface IAimProvider
    {
        /// <summary>Normalized direction on the XZ plane.</summary>
        Vector3 AimDirection { get; }

        /// <summary>The aimed point on the floor plane.</summary>
        Vector3 AimPoint { get; }
    }
}
