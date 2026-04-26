using UnityEngine;

public abstract class EnemyAttack : MonoBehaviour
{
    [SerializeField] protected EnemyStats _stats;
    [SerializeField] protected SoundEffect _attackSound;

    private float _lastAttackTime = float.MinValue;

    public void ExecuteAttack()
    {
        if (CanAttack())
        {
            Attack();
            _attackSound?.Play();
            _lastAttackTime = Time.time;
        }
    }

    private bool CanAttack()
    {
        return Time.time >= _lastAttackTime + _stats.AttackCooldown;
    }

    protected abstract void Attack();
}