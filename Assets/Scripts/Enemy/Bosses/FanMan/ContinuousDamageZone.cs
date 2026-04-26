using System.Collections;
using Player;
using UnityEngine;

namespace Enemy.Bosses.FanMan
{
    /// <summary>
    /// Wraps a DamageDealer to apply continuous damage on a cooldown cycle.
    /// Useful for lingering beams and fan push zones.
    /// </summary>
    public class ContinuousDamageZone : MonoBehaviour
    {
        [SerializeField] private float _hitCooldown = 0.5f;

        private DamageDealer _damageDealer;
        private Coroutine _cooldownRoutine;
        private int _damage;
        private Vector2 _knockback;
        private float _knockbackDuration;

        private void Awake()
        {
            _damageDealer = GetComponent<DamageDealer>();
            if (_damageDealer == null)
            {
                _damageDealer = gameObject.AddComponent<DamageDealer>();
            }
            _damageDealer.OnHit += HandleHit;
        }

        private void OnDestroy()
        {
            if (_damageDealer != null)
            {
                _damageDealer.OnHit -= HandleHit;
            }
        }

        private void OnDisable()
        {
            if (_cooldownRoutine != null)
            {
                StopCoroutine(_cooldownRoutine);
                _cooldownRoutine = null;
            }
            _damageDealer?.Deactivate();
        }

        public void Configure(int damage, Vector2 knockback, float knockbackDuration = -1f)
        {
            _damage = damage;
            _knockback = knockback;
            _knockbackDuration = knockbackDuration;
        }

        public void Activate()
        {
            if (_cooldownRoutine != null)
            {
                StopCoroutine(_cooldownRoutine);
                _cooldownRoutine = null;
            }
            _damageDealer.Activate(new DamageInfo(_damage, _knockback, _knockbackDuration));
        }

        public void Deactivate()
        {
            if (_cooldownRoutine != null)
            {
                StopCoroutine(_cooldownRoutine);
                _cooldownRoutine = null;
            }
            _damageDealer.Deactivate();
        }

        private void HandleHit()
        {
            if (_cooldownRoutine != null)
            {
                StopCoroutine(_cooldownRoutine);
            }
            _cooldownRoutine = StartCoroutine(CooldownRoutine());
        }

        private IEnumerator CooldownRoutine()
        {
            _damageDealer.Deactivate();
            yield return new WaitForSeconds(_hitCooldown);
            if (gameObject.activeInHierarchy)
            {
                _damageDealer.Activate(new DamageInfo(_damage, _knockback, _knockbackDuration));
            }
            _cooldownRoutine = null;
        }
    }
}
