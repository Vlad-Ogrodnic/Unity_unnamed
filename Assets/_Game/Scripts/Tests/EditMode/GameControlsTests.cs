using Game.Player;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Tests.EditMode
{
    /// <summary>Checks the Gameplay action map against the controls table in GDD ch. 3.</summary>
    public class GameControlsTests : InputTestFixture
    {
        private GameControls _controls;
        private Keyboard _keyboard;
        private Mouse _mouse;

        public override void Setup()
        {
            base.Setup();
            _keyboard = InputSystem.AddDevice<Keyboard>();
            _mouse = InputSystem.AddDevice<Mouse>();
            _controls = new GameControls();
            _controls.Gameplay.Enable();
        }

        public override void TearDown()
        {
            // The generated Dispose() calls Object.Destroy, which is not allowed in Edit Mode.
            _controls.Disable();
            Object.DestroyImmediate(_controls.asset);
            base.TearDown();
        }

        [TestCase("Move")]
        [TestCase("Aim")]
        [TestCase("Fire")]
        [TestCase("Melee")]
        [TestCase("Bomb")]
        [TestCase("Potion")]
        [TestCase("Interact")]
        [TestCase("Dash")]
        [TestCase("Pause")]
        public void GameplayMap_HasAction(string actionName)
        {
            Assert.That(_controls.Gameplay.Get().FindAction(actionName), Is.Not.Null);
        }

        [Test]
        public void Move_Diagonal_IsNormalized()
        {
            Press(_keyboard.wKey);
            Press(_keyboard.dKey);

            Vector2 move = _controls.Gameplay.Move.ReadValue<Vector2>();

            Assert.That(move.magnitude, Is.EqualTo(1f).Within(1e-4f));
            Assert.That(move.x, Is.GreaterThan(0f));
            Assert.That(move.y, Is.GreaterThan(0f));
        }

        [Test]
        public void Move_Released_IsZero()
        {
            Press(_keyboard.aKey);
            Release(_keyboard.aKey);

            Assert.That(_controls.Gameplay.Move.ReadValue<Vector2>(), Is.EqualTo(Vector2.zero));
        }

        [Test]
        public void Aim_ReadsMousePosition()
        {
            Set(_mouse.position, new Vector2(123f, 45f));

            Assert.That(_controls.Gameplay.Aim.ReadValue<Vector2>(), Is.EqualTo(new Vector2(123f, 45f)));
        }

        [Test]
        public void Fire_IsPressedWhileLeftMouseHeld()
        {
            Press(_mouse.leftButton);
            Assert.That(_controls.Gameplay.Fire.IsPressed(), Is.True);

            Release(_mouse.leftButton);
            Assert.That(_controls.Gameplay.Fire.IsPressed(), Is.False);
        }

        [Test]
        public void Bomb_ReportsPressAndRelease()
        {
            Press(_keyboard.eKey);
            Assert.That(_controls.Gameplay.Bomb.WasPressedThisFrame(), Is.True);

            Release(_keyboard.eKey);
            Assert.That(_controls.Gameplay.Bomb.WasReleasedThisFrame(), Is.True);
        }

        [Test]
        public void SingleKeyActions_UseGddKeys()
        {
            AssertTriggers(_mouse.rightButton, _controls.Gameplay.Melee);
            AssertTriggers(_keyboard.qKey, _controls.Gameplay.Potion);
            AssertTriggers(_keyboard.fKey, _controls.Gameplay.Interact);
            AssertTriggers(_keyboard.spaceKey, _controls.Gameplay.Dash);
            AssertTriggers(_keyboard.escapeKey, _controls.Gameplay.Pause);
        }

        private void AssertTriggers(UnityEngine.InputSystem.Controls.ButtonControl button, InputAction action)
        {
            Press(button);
            Assert.That(action.WasPressedThisFrame(), Is.True, $"{button.path} should trigger {action.name}");
            Release(button);
        }
    }
}
