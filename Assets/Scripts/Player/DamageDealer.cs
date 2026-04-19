using System;
using System.Collections.Generic;
using UnityEngine;

namespace Player
{
    [RequireComponent(typeof(Collider2D))]
    public class DamageDealer : MonoBehaviour
    {
        private int _damageAmount;
        private bool _active;
        private readonly HashSet<Collider2D> _hitTargets = new();

        public event Action OnHit;

        public void Activate(int damage)
        {
            _damageAmount = damage;
            _active = true;
            _hitTargets.Clear();
        }

        public void Deactivate()
        {
            _active = false;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!_active)
            {
                return;
            }

            if (_hitTargets.Contains(other))
            {
                return;
            }

            var target = other.gameObject.GetComponent<IDamageable>();
            if (target != null)
            {
                target.TakeDamage(_damageAmount);
                _hitTargets.Add(other);
                OnHit?.Invoke();
            }
        }
    }
}