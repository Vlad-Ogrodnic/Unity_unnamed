using Game.Core;
using UnityEngine;

namespace Game.Enemies
{
    /// <summary>Walks straight at its target. The room (or the scene, for now) gives it the target.</summary>
    [RequireComponent(typeof(Rigidbody))]
    public sealed class ChaserBehaviour : MonoBehaviour
    {
        private enum State
        {
            Idle,
            Chase
        }

        [SerializeField] private EnemyConfig _config;
        [SerializeField] private Health _health;
        [SerializeField] private Transform _target;

        private readonly KnockbackState _knockback = new KnockbackState();
        private Rigidbody _body;
        private State _state;

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
        }

        private void OnDisable()
        {
            _health.Damaged -= OnDamaged;
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

            _state = _target != null ? State.Chase : State.Idle;

            Vector3 velocity = Vector3.zero;
            switch (_state)
            {
                case State.Chase:
                    velocity = PlanarMath.Direction(_body.position, _target.position, Vector3.zero) * _config.MoveSpeed;
                    break;
            }

            _body.linearVelocity = velocity;
        }

        private void OnDamaged(DamageInfo damage)
        {
            _knockback.Apply(damage.Direction, damage.KnockbackForce, _config.KnockbackDuration);
        }
    }
}
