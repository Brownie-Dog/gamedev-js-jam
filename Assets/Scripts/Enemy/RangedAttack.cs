using UnityEngine;

public class RangedAttack : EnemyAttack
{
    [SerializeField] private Transform _player;

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
    }
}
