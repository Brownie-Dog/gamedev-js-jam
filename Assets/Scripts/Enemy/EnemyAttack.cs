using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    [Header("Attack Settings")] [SerializeField]
    private float _attackCooldown = 1f;

    private float _lastAttackTime;

    public bool CanAttack()
    {
        return Time.time >= _lastAttackTime + _attackCooldown;
    }

    public void ResetCooldown()
    {
        _lastAttackTime = Time.time;
    }
}
