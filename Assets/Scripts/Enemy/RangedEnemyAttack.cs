using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Pool;
using Weapons;

public class RangedEnemyAttack : EnemyAttack
{
    [SerializeField] private Bullet _bulletPrefab;
    [SerializeField] private int _poolSize = 15;
    [SerializeField] private Transform _firePoint;

    private Transform _player;
    private IObjectPool<Bullet> _bulletPool;

    private void Awake()
    {
        _player = GameObject.FindGameObjectWithTag("Player").transform;
        Assert.IsNotNull(_player);
        Assert.IsNotNull(_bulletPrefab);
        Assert.IsNotNull(_firePoint);

        _bulletPool = new ObjectPool<Bullet>(CreateBullet, OnGetBullet, OnReleaseBullet, OnDestroyBullet,
            defaultCapacity: _poolSize, maxSize: _poolSize
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
        var direction = ((Vector2)(_player.position - _firePoint.position)).normalized;
        var damageInfo = new DamageInfo(_stats.Damage, direction * _stats.KnockbackForce);
        var bullet = _bulletPool.Get();
        bullet.Activate(_firePoint.position, direction, damageInfo);
    }

    private Bullet CreateBullet()
    {
        var bullet = Instantiate(_bulletPrefab, transform);
        bullet.SetPool(_bulletPool);
        return bullet;
    }

    private void OnGetBullet(Bullet bullet)
    {
        bullet.gameObject.SetActive(true);
    }

    private void OnReleaseBullet(Bullet bullet)
    {
        bullet.Deactivate();
        bullet.gameObject.SetActive(false);
    }

    private void OnDestroyBullet(Bullet bullet)
    {
        Destroy(bullet.gameObject);
    }
}