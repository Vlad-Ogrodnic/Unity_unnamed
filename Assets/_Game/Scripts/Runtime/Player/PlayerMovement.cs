using Game.Core;
using UnityEngine;

namespace Game.Player
{
    /// <summary>
    /// Moves the player on the XZ plane by setting the Rigidbody's velocity.
    /// The Rigidbody must freeze rotation and Y position (set on the prefab).
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public sealed class PlayerMovement : MonoBehaviour
    {
        [SerializeField] private PlayerConfig _config;
        [SerializeField] private PlayerInputReader _input;
        [SerializeField] private Health _health;

        private readonly KnockbackState _knockback = new KnockbackState();
        private Rigidbody _body;

        private void Awake()
        {
            _body = GetComponent<Rigidbody>();
        }

        private void OnEnable()
        {
            if (_health != null)
                _health.Damaged += OnDamaged;
        }

        private void OnDisable()
        {
            if (_health != null)
                _health.Damaged -= OnDamaged;

            _knockback.Clear();
            _body.linearVelocity = Vector3.zero;
        }

        private void FixedUpdate()
        {
            Vector3 velocity;
            if (_knockback.IsActive)
            {
                velocity = _knockback.Velocity;
                _knockback.Tick(Time.fixedDeltaTime);
            }
            else
            {
                Vector2 move = _input.Move;
                velocity = new Vector3(move.x, 0f, move.y) * _config.MoveSpeed;
            }

            _body.linearVelocity = velocity;
        }

        private void OnDamaged(DamageInfo damage)
        {
            _knockback.Apply(damage.Direction, damage.KnockbackForce, _config.KnockbackDuration);
        }
    }
}
