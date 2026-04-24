using System;
using UnityEngine;
using UnityEngine.Assertions;
using Random = UnityEngine.Random;

public class EnemyMovement : MonoBehaviour
{
    [SerializeField] private Rigidbody2D _rb;
    [SerializeField] private EnemyDetection _enemyDetection;
    [SerializeField] private EnemyStats _stats;

    private Transform _player;
    private bool _isChasing = false;
    private Vector2 _wanderDirection;
    private float _wanderTimer;

    private void Awake()
    {
        _player = GameObject.FindGameObjectWithTag("Player").transform;
        Assert.IsNotNull(_player);
        Assert.IsNotNull(_enemyDetection);
        PickNewWanderDirection();
    }

    private void OnEnable()
    {
        _enemyDetection.OnPlayerDetected += HandlePlayerDetected;
        _enemyDetection.OnPlayerLost += HandlePlayerLost;
    }

    private void OnDisable()
    {
        _enemyDetection.OnPlayerDetected -= HandlePlayerDetected;
        _enemyDetection.OnPlayerLost -= HandlePlayerLost;
    }

    private void FixedUpdate()
    {
        if (_isChasing)
        {
            float distanceToPlayer = Vector2.Distance(_player.position, transform.position);
            if (distanceToPlayer > _stats.StoppingDistance)
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
            Wander();
        }
    }

    private void HandlePlayerDetected(object sender, EventArgs e)
    {
        _isChasing = true;
    }

    private void HandlePlayerLost(object sender, EventArgs e)
    {
        _isChasing = false;
        PickNewWanderDirection();
    }

    protected virtual void ChasePlayer()
    {
        Vector2 direction = ((Vector2)_player.position - (Vector2)transform.position).normalized;
        _rb.linearVelocity = direction * _stats.MovementSpeed;
    }

    protected virtual void Wander()
    {
        _wanderTimer -= Time.fixedDeltaTime;

        if (_wanderTimer <= 0f)
        {
            PickNewWanderDirection();
        }

        _rb.linearVelocity = _wanderDirection * _stats.WanderSpeed;
    }

    private void PickNewWanderDirection()
    {
        _wanderDirection = Random.insideUnitCircle.normalized;
        _wanderTimer = _stats.WanderDirectionChangeInterval;
    }
}