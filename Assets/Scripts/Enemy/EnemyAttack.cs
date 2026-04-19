using UnityEngine;

public abstract class EnemyAttack : MonoBehaviour
{
    [SerializeField] protected EnemyStats _stats;
    [SerializeField] protected SoundEffect _attackSound;

    private float _lastAttackTime;

    public void ExecuteAttack()
    {
        if (CanAttack())
        {
            Attack();
            _attackSound?.Play();
            ResetCooldown();
        }
    }

    protected bool CanAttack()
    {
        return Time.time >= _lastAttackTime + _stats.AttackCooldown;
    }

    protected abstract void Attack();

    private void ResetCooldown()
    {
        _lastAttackTime = Time.time;
    }
}