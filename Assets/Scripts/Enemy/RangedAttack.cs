using UnityEngine;
using UnityEngine.Assertions;

public class RangedAttack : EnemyAttack
{
    private Transform _player;
    [SerializeField] private Transform _firePoint;
    [SerializeField] private ObjectPooler _bulletPool;

    
    private void Awake()
    {
        _player = GameObject.FindGameObjectWithTag("Player").transform;
        Assert.IsNotNull(_player);
    }
    private void Update()
    {
        if (!_player) return;

        float distance = Vector2.Distance(transform.position, _player.position);
        
        if (distance <= _stats.attackRange)
        {
            ExecuteAttack();
        }
    }

    protected override void Attack()
    {
        Debug.Log("Firing Projectile!");

        // bullet or projectile
        // do damage
        if (!_player) return;
        Projectile bullet = _bulletPool.Get();
        bullet.transform.position = _firePoint.position;
        Vector2 targetDirection = (_player.position - _firePoint.position).normalized;
        bullet.Launch(targetDirection);
    }
}
