using System;
using System.Collections.Generic;
using UnityEngine;

namespace Player
{
    [RequireComponent(typeof(Collider2D))]
    public class DamageDealer : MonoBehaviour
    {
        [SerializeField] private LayerMask _targetLayerMask;

        private int _damageAmount;
        private Vector2 _knockback;
        private bool _useExplicitKnockback;
        private bool _active;
        private readonly HashSet<GameObject> _hitTargets = new();

        public event Action OnHit;

        private void Awake()
        {
            if (_targetLayerMask == LayerMask.NameToLayer(GlobalConstants.DEFAULT_LAYER))
            {
                _targetLayerMask = LayerMask.GetMask(GlobalConstants.ENEMY_LAYER);
            }
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            TryDealDamage(other);
        }

        public void Activate(DamageInfo damageInfo, bool useExplicitKnockback = true)
        {
            _damageAmount = damageInfo.Damage;
            _knockback = damageInfo.Knockback;
            _useExplicitKnockback = useExplicitKnockback && damageInfo.Knockback.sqrMagnitude > 0.001f;
            _active = true;
            _hitTargets.Clear();
        }

        public void Deactivate()
        {
            _active = false;
        }

        private void TryDealDamage(Collider2D other)
        {
            if (!_active)
            {
                return;
            }

            if (((1 << other.gameObject.layer) & _targetLayerMask) == 0)
            {
                return;
            }

            if (_hitTargets.Contains(other.gameObject))
            {
                return;
            }

            var target = other.gameObject.GetComponent<IDamageable>();
            if (target != null)
            {
                Vector2 knockback;
                if (_useExplicitKnockback)
                {
                    knockback = _knockback;
                }
                else
                {
                    var direction = ((Vector2)(other.transform.position - transform.position)).normalized;
                    knockback = direction * _knockback.magnitude;
                }

                var info = new DamageInfo(_damageAmount, knockback);
                target.TakeDamage(info);
                _hitTargets.Add(other.gameObject);
                OnHit?.Invoke();
            }
        }
    }
}
