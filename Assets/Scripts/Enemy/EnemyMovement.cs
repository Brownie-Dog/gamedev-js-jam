using System;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [Header("References")] 
    [SerializeField] private Transform _player;
    [SerializeField] private Rigidbody2D _rb;
    [SerializeField] private EnemyDetection _enemyDetection;
    [SerializeField] private EnemyStats _stats;
    
    private bool _isChasing = false;

    private void OnEnable()
    {
        if (_enemyDetection !=  null)
        {
            _enemyDetection.OnPlayerDetected += HandlePlayerDetected;
            _enemyDetection.OnPlayerLost += HandlePlayerLost;
        }
        
    }
    private void OnDisable()
    {
        if (_enemyDetection != null)
        {
            _enemyDetection.OnPlayerDetected -= HandlePlayerDetected;
            _enemyDetection.OnPlayerLost -= HandlePlayerLost;
        }
    }
    private void Update()
    {
        if (_isChasing && _player)
        {
            float distanceToPlayer = Vector2.Distance(_player.position, transform.position);
            if (distanceToPlayer > _stats.stoppingDistance)
            {
                ChasePlayer();
            }
            else
            {
                _rb.linearVelocity = Vector2.zero;
            }
        }
        else
        {
            _rb.linearVelocity = Vector2.zero;
        }
    }

    private void HandlePlayerDetected(object sender, EventArgs e)
    {
        _isChasing = true;
    }

    private void HandlePlayerLost(object sender, EventArgs e)
    {
        _isChasing = false;
    }
    
    private void ChasePlayer()
    {
        if (!_player) return;
        
        Vector2 direction = ((Vector2)_player.position - (Vector2)transform.position).normalized;
        _rb.linearVelocity = direction * _stats.movementSpeed;
    }

}
