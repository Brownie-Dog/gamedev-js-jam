using UnityEngine;
using UnityEngine.Assertions;

public class RangedEnemyAttack : EnemyAttack
{
    [SerializeField] private Transform _firePoint;
    [SerializeField] private EnemyProjectilePooler _bulletPool;
    
    private Transform _player;
    
    private void Awake()
    {
        _player = GameObject.FindGameObjectWithTag("Player").transform;
        Assert.IsNotNull(_player);
        Assert.IsNotNull(_bulletPool);
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
        Debug.Log("Firing Projectile!");
        PooledEnemyProjectile bullet = _bulletPool.RequestProjectile();
        bullet.transform.position = _firePoint.position;
        Vector2 targetDirection = (_player.position - _firePoint.position).normalized;
        bullet.Launch(targetDirection);
    }
}
