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
        [SerializeField] private MeleeAttack _melee;

        private void OnEnable() => _input.MeleePressed += OnMeleePressed;

        private void OnDisable()
        {
            _input.MeleePressed -= OnMeleePressed;
            if (_ranged != null)
                _ranged.TriggerHeld = false;
        }

        private void Update()
        {
            _ranged.TriggerHeld = _input.FireHeld;
        }

        private void OnMeleePressed() => _melee.TryAttack();
    }
}
