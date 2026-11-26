using Game.Combat;
using Game.Core;
using UnityEngine;

namespace Game.Enemies
{
    /// <summary>
    /// Stays between a minimum and maximum distance from the target and shoots at it.
    /// It is also the aim source for its own <see cref="RangedAttack"/>.
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public sealed class ShooterBehaviour : MonoBehaviour, IAimProvider, IEnemyBehaviour
    {
        private enum State
        {
            Idle,
            Approach,
            Hold,
            Retreat
        }

        [SerializeField] private ShooterConfig _config;
        [SerializeField] private Health _health;
        [SerializeField] private RangedAttack _ranged;
        [SerializeField] private Transform _target;

        private readonly KnockbackState _knockback = new KnockbackState();
        private Rigidbody _body;
        private State _state;

        public Vector3 AimDirection { get; private set; } = Vector3.forward;
        public Vector3 AimPoint => _target != null ? _target.position : transform.position + AimDirection;

        public void SetTarget(Transform target)
        {
            _target = target;
        }

        private void Awake()
        {
            _body = GetComponent<Rigidbody>();
        }

        private void OnEnable() => _health.Damaged += OnDamaged;

        private void OnDisable()
        {
            _health.Damaged -= OnDamaged;
            _knockback.Clear();
            _ranged.TriggerHeld = false;
            _body.linearVelocity = Vector3.zero;
        }

        private void Update()
        {
            if (_target == null)
            {
                _ranged.TriggerHeld = false;
                return;
            }

            AimDirection = PlanarMath.Direction(transform.position, _target.position, AimDirection);
            float distance = Vector3.Distance(PlanarMath.Flatten(transform.position), PlanarMath.Flatten(_target.position));
            _ranged.TriggerHeld = distance <= _config.FireDistance;
        }

        private void FixedUpdate()
        {
            if (_knockback.IsActive)
            {
                _body.linearVelocity = _knockback.Velocity;
                _knockback.Tick(Time.fixedDeltaTime);
                return;
            }

            _state = ChooseState();
            Vector3 toTarget = _target != null ? PlanarMath.Direction(_body.position, _target.position, Vector3.zero) : Vector3.zero;

            Vector3 velocity = Vector3.zero;
            switch (_state)
            {
                case State.Approach:
                    velocity = toTarget * _config.MoveSpeed;
                    break;
                case State.Retreat:
                    velocity = -toTarget * _config.MoveSpeed;
                    break;
            }

            _body.linearVelocity = velocity;
        }

        private State ChooseState()
        {
            if (_target == null)
                return State.Idle;

            float distance = Vector3.Distance(PlanarMath.Flatten(_body.position), PlanarMath.Flatten(_target.position));
            if (distance < _config.MinDistance)
                return State.Retreat;
            if (distance > _config.MaxDistance)
                return State.Approach;
            return State.Hold;
        }

        private void OnDamaged(DamageInfo damage)
        {
            _knockback.Apply(damage.Direction, damage.KnockbackForce, _config.KnockbackDuration);
        }
    }
}
