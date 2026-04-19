using UnityEngine;
using UnityEngine.Assertions;

public class RangedEnemyAttack : EnemyAttack
{
    [SerializeField] private Transform _firePoint;
    [SerializeField] private ObjectPooler _bulletPool;
    
    private Transform _player;
    
    private void Awake()
    {
        _player = GameObject.FindGameObjectWithTag("Player").transform;
        Assert.IsNotNull(_player);
    }
    private void Update()
    {
        float distance = Vector2.Distance(transform.position, _player.position);
        
        if (distance <= _stats.attackRange)
        {
            ExecuteAttack();
        }
    }

    protected override void Attack()
    {
        Debug.Log("Firing Projectile!");
        if (!_player) return;
        Projectile bullet = _bulletPool.Get();
        bullet.transform.position = _firePoint.position;
        Vector2 targetDirection = (_player.position - _firePoint.position).normalized;
        bullet.Launch(targetDirection);
    }
}
