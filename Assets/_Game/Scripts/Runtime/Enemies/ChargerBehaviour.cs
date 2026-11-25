using System;
using Game.Core;
using UnityEngine;

namespace Game.Enemies
{
    /// <summary>
    /// Wait → Telegraph (blinks; direction locked at the start so the player can dodge) →
    /// Charge in a straight line until it hits a wall or obstacle → Recover → Wait.
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public sealed class ChargerBehaviour : MonoBehaviour
    {
        private enum State
        {
            Wait,
            Telegraph,
            Charge,
            Recover
        }

        // A wall stops the charge only if it is hit roughly head-on, not grazed from the side.
        private const float HeadOnDot = -0.5f;

        [SerializeField] private ChargerConfig _config;
        [SerializeField] private Health _health;
        [SerializeField] private Transform _target;

        private readonly KnockbackState _knockback = new KnockbackState();
        private Rigidbody _body;
        private State _state = State.Wait;
        private float _stateEndTime;
        private Vector3 _chargeDirection;

        /// <summary>For visuals: the telegraph starts. Carries the locked charge direction.</summary>
        public event Action<Vector3> TelegraphStarted;

        /// <summary>For visuals: blinking ends (charge starts, or the telegraph was interrupted).</summary>
        public event Action TelegraphEnded;

        public void SetTarget(Transform target)
        {
            _target = target;
        }

        private void Awake()
        {
            _body = GetComponent<Rigidbody>();
        }

        private void OnEnable()
        {
            _health.Damaged += OnDamaged;
            Enter(State.Wait, _config.WaitDuration);
        }

        private void OnDisable()
        {
            _health.Damaged -= OnDamaged;
            if (_state == State.Telegraph)
                TelegraphEnded?.Invoke();
            _knockback.Clear();
            _body.linearVelocity = Vector3.zero;
        }

        private void FixedUpdate()
        {
            if (_knockback.IsActive)
            {
                _body.linearVelocity = _knockback.Velocity;
                _knockback.Tick(Time.fixedDeltaTime);
                return;
            }

            float now = Time.time;
            switch (_state)
            {
                case State.Wait:
                    if (now >= _stateEndTime && _target != null)
                    {
                        _chargeDirection = PlanarMath.Direction(_body.position, _target.position, transform.forward);
                        Enter(State.Telegraph, _config.TelegraphDuration);
                    }
                    break;
                case State.Telegraph:
                    if (now >= _stateEndTime)
                        Enter(State.Charge, _config.MaxChargeDuration);
                    break;
                case State.Charge:
                    if (now >= _stateEndTime)
                        Enter(State.Recover, _config.RecoverDuration);
                    break;
                case State.Recover:
                    if (now >= _stateEndTime)
                        Enter(State.Wait, _config.WaitDuration);
                    break;
            }

            _body.linearVelocity = _state == State.Charge ? _chargeDirection * _config.ChargeSpeed : Vector3.zero;
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (_state != State.Charge || collision.collider.GetComponentInParent<IDamageable>() != null)
                return;

            for (int i = 0; i < collision.contactCount; i++)
            {
                if (Vector3.Dot(collision.GetContact(i).normal, _chargeDirection) <= HeadOnDot)
                {
                    Enter(State.Recover, _config.RecoverDuration);
                    return;
                }
            }
        }

        private void Enter(State state, float duration)
        {
            if (_state == State.Telegraph && state != State.Telegraph)
                TelegraphEnded?.Invoke();

            _state = state;
            _stateEndTime = Time.time + duration;

            if (state == State.Telegraph)
                TelegraphStarted?.Invoke(_chargeDirection);
        }

        private void OnDamaged(DamageInfo damage)
        {
            _knockback.Apply(damage.Direction, damage.KnockbackForce, _config.KnockbackDuration);

            // Being knocked back ends a charge in progress.
            if (_knockback.IsActive && _state == State.Charge)
                Enter(State.Recover, _config.RecoverDuration);
        }
    }
}
