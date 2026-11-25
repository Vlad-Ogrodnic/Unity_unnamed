using Game.Combat;
using UnityEngine;

namespace Game.Player
{
    /// <summary>Connects player input to the attacks, which themselves know nothing about input.</summary>
    [DefaultExecutionOrder(-40)]
    public sealed class PlayerAttackInput : MonoBehaviour
    {
        [SerializeField] private PlayerInputReader _input;
        [SerializeField] private RangedAttack _ranged;

        private void Update()
        {
            _ranged.TriggerHeld = _input.FireHeld;
        }

        private void OnDisable()
        {
            if (_ranged != null)
                _ranged.TriggerHeld = false;
        }
    }
}
