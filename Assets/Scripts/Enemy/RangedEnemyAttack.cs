using UnityEngine;
using UnityEngine.Assertions;
using Weapons;

public class RangedEnemyAttack : EnemyAttack
{
    [SerializeField] private Bullet _bulletPrefab;
    [SerializeField] private int _poolSize = 15;
    [SerializeField] private Transform _firePoint;

    private Transform _player;
    private MonoBehaviourPool<Bullet> _bulletPool;

    private void Awake()
    {
        _player = GameObject.FindGameObjectWithTag("Player").transform;
        Assert.IsNotNull(_player);
        Assert.IsNotNull(_bulletPrefab);
        Assert.IsNotNull(_firePoint);

        _bulletPool = new MonoBehaviourPool<Bullet>(_bulletPrefab, _poolSize, _poolSize, transform,
            b => b.SetPool(_bulletPool.Pool)
        );
    }

    private void Update()
    {
        float distance = Vector2.Distance(transform.position, _player.position);

        if (distance <= _stats.AttackRange)
        {
            ExecuteAttack();
        }
    }

    protected override void Attack()
    {
        Vector2 direction = ((Vector2)(_player.position - _firePoint.position)).normalized;
        var damageInfo = new DamageInfo(_stats.Damage, direction * _stats.KnockbackForce);
        Bullet bullet = _bulletPool.Get();
        bullet.Activate(_firePoint.position, direction, damageInfo);
    }
}