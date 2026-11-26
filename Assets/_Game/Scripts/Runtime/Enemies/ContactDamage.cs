using Game.Core;
using UnityEngine;

namespace Game.Enemies
{
    /// <summary>
    /// Damages whatever it touches on the other team. Called on every contact frame;
    /// the target's i-frames stop repeated hits.
    /// </summary>
    public sealed class ContactDamage : MonoBehaviour
    {
        [SerializeField] private EnemyConfig _config;
        [SerializeField] private Health _health;

        private void OnCollisionEnter(Collision collision) => TryDamage(collision.collider);

        private void OnCollisionStay(Collision collision) => TryDamage(collision.collider);

        private void TryDamage(Collider other)
        {
            if (!enabled || _health.IsDead)
                return;

            IDamageable target = other.GetComponentInParent<IDamageable>();
            if (target == null || target.Team == _health.Team)
                return;

            Vector3 away = other.transform.position - transform.position;
            target.TryTakeDamage(new DamageInfo(_config.ContactDamage, away, _config.ContactKnockbackForce, gameObject));
        }
    }
}
