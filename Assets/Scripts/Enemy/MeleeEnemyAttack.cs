using UnityEngine;
using UnityEngine.Assertions;
using Player;

[RequireComponent(typeof(DamageDealer))]
public class MeleeEnemyAttack : EnemyAttack
{
    private DamageDealer _damageDealer;
    private Collider2D _playerCollider;

    private void Awake()
    {
        _damageDealer = GetComponent<DamageDealer>();
        Assert.IsNotNull(_damageDealer);

        _damageDealer.OnHit += OnEnemyHit;
    }

    private void OnEnemyHit()
    {
        _damageDealer.Deactivate();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(GlobalConstants.PLAYER_TAG))
        {
            return;
        }

        _playerCollider = other;
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!other.CompareTag(GlobalConstants.PLAYER_TAG))
        {
            return;
        }

        ExecuteAttack();
    }

    protected override void Attack()
    {
        if (_playerCollider == null) return;
        var direction = ((Vector2)(_playerCollider.transform.position - transform.position)).normalized;
        var damageInfo = new DamageInfo(_stats.Damage, direction * _stats.KnockbackForce);
        _damageDealer.Activate(damageInfo);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag(GlobalConstants.PLAYER_TAG))
        {
            _playerCollider = null;
            _damageDealer?.Deactivate();
        }
    }
}