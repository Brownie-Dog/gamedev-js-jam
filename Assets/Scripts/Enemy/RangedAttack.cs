using UnityEngine;

public class RangedAttack : EnemyAttack
{
    [Header("Attack Settings")]
    [SerializeField] private float _attackRange = 8f;
    [SerializeField] private Transform _player;
    [Header("Sound Effect")]
    [SerializeField] private SoundEffect _attackSound;
    
    private void Update()
    {
        if (!_player) return;
        
        float distance = Vector2.Distance(transform.position, _player.position);
        
        if (distance <= _attackRange && CanAttack())
        {
            _attackSound?.Play();
            ResetCooldown();
        }
    }
}
