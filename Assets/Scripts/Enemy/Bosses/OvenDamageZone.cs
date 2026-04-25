using System.Collections;
using Player;
using UnityEngine;
using UnityEngine.Assertions;

namespace Enemy.Bosses
{
    public class OvenDamageZone : MonoBehaviour
    {
        [SerializeField] private int _damage = 3;
        [SerializeField] private float _hitCooldown = 1f;

        private DamageDealer _damageDealer;
        private Coroutine _cooldownRoutine;

        private void Awake()
        {
            _damageDealer = GetComponent<DamageDealer>();
            Assert.IsNotNull(_damageDealer);

            _damageDealer.OnHit += HandleHit;
            _damageDealer.Activate(new DamageInfo(_damage, Vector2.zero));
        }

        private void OnDestroy()
        {
            if (_damageDealer != null)
            {
                _damageDealer.OnHit -= HandleHit;
            }
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
            _damageDealer.Activate(new DamageInfo(_damage, Vector2.zero));
            _cooldownRoutine = null;
        }
    }
}
