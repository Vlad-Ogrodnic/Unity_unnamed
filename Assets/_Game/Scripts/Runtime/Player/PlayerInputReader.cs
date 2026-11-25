using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Player
{
    /// <summary>
    /// The only place that reads the Input System. Other components read its values
    /// and events and never touch input directly.
    /// </summary>
    [DefaultExecutionOrder(-100)]
    public sealed class PlayerInputReader : MonoBehaviour
    {
        private GameControls _controls;

        public Vector2 Move { get; private set; }
        public Vector2 AimScreenPosition { get; private set; }
        public bool FireHeld { get; private set; }

        public event Action MeleePressed;
        public event Action BombPressed;
        public event Action BombReleased;
        public event Action PotionPressed;
        public event Action InteractPressed;
        public event Action DashPressed;
        public event Action PausePressed;

        /// <summary>Provisional (M2-02): R on the temporary death screen. Removed with the real end screen (M4-07).</summary>
        public event Action RestartPressed;

        private void Awake()
        {
            _controls = new GameControls();
        }

        private void OnEnable()
        {
            GameControls.GameplayActions gameplay = _controls.Gameplay;
            gameplay.Melee.performed += OnMelee;
            gameplay.Bomb.performed += OnBombPressed;
            gameplay.Bomb.canceled += OnBombReleased;
            gameplay.Potion.performed += OnPotion;
            gameplay.Interact.performed += OnInteract;
            gameplay.Dash.performed += OnDash;
            gameplay.Pause.performed += OnPause;
            gameplay.Restart.performed += OnRestart;
            gameplay.Enable();
        }

        private void OnDisable()
        {
            GameControls.GameplayActions gameplay = _controls.Gameplay;
            gameplay.Disable();
            gameplay.Melee.performed -= OnMelee;
            gameplay.Bomb.performed -= OnBombPressed;
            gameplay.Bomb.canceled -= OnBombReleased;
            gameplay.Potion.performed -= OnPotion;
            gameplay.Interact.performed -= OnInteract;
            gameplay.Dash.performed -= OnDash;
            gameplay.Pause.performed -= OnPause;
            gameplay.Restart.performed -= OnRestart;

            Move = Vector2.zero;
            FireHeld = false;
        }

        private void OnDestroy()
        {
            _controls?.Dispose();
        }

        private void Update()
        {
            GameControls.GameplayActions gameplay = _controls.Gameplay;
            Move = Vector2.ClampMagnitude(gameplay.Move.ReadValue<Vector2>(), 1f);
            AimScreenPosition = gameplay.Aim.ReadValue<Vector2>();
            FireHeld = gameplay.Fire.IsPressed();
        }

        private void OnMelee(InputAction.CallbackContext _) => MeleePressed?.Invoke();
        private void OnBombPressed(InputAction.CallbackContext _) => BombPressed?.Invoke();
        private void OnBombReleased(InputAction.CallbackContext _) => BombReleased?.Invoke();
        private void OnPotion(InputAction.CallbackContext _) => PotionPressed?.Invoke();
        private void OnInteract(InputAction.CallbackContext _) => InteractPressed?.Invoke();
        private void OnDash(InputAction.CallbackContext _) => DashPressed?.Invoke();
        private void OnPause(InputAction.CallbackContext _) => PausePressed?.Invoke();
        private void OnRestart(InputAction.CallbackContext _) => RestartPressed?.Invoke();
    }
}
