using System;
using UnityEngine;
using UnityEngine.Assertions;
using Random = UnityEngine.Random;

public class EnemyMovement : MonoBehaviour
{
    [SerializeField] protected Rigidbody2D _rb;
    [SerializeField] private EnemyDetection _enemyDetection;
    [SerializeField] protected EnemyStats _stats;

    protected enum WanderState
    {
        Moving,
        Idling
    }

    protected Transform _player;
    protected bool _isChasing = false;
    protected bool _isMovementPaused = false;
    protected Vector2 _wanderDirection;
    protected float _wanderMoveTimer;
    protected float _wanderIdleTimer;
    protected WanderState _wanderState = WanderState.Moving;

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
        if (_isMovementPaused)
        {
            _rb.linearVelocity = Vector2.zero;
            return;
        }

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

    private void OnCollisionEnter2D(Collision2D col)
    {
        if ((_stats.WallLayer & (1 << col.gameObject.layer)) != 0 && !_isChasing)
        {
            PickNewWanderDirection();
            _wanderState = WanderState.Moving;
        }
    }

    protected virtual void HandlePlayerDetected(object sender, EventArgs e)
    {
        _isChasing = true;
    }

    protected virtual void HandlePlayerLost(object sender, EventArgs e)
    {
        _isChasing = false;
        PickNewWanderDirection();
    }

    public void PauseMovement()
    {
        _isMovementPaused = true;
    }

    public void ResumeMovement()
    {
        _isMovementPaused = false;
    }

    protected virtual void ChasePlayer()
    {
        Vector2 direction = ((Vector2)_player.position - (Vector2)transform.position).normalized;
        _rb.linearVelocity = direction * _stats.MovementSpeed;
    }

    protected virtual void Wander()
    {
        switch (_wanderState)
        {
            case WanderState.Moving:
                _wanderMoveTimer -= Time.fixedDeltaTime;
                _rb.linearVelocity = _wanderDirection * _stats.WanderSpeed;

                if (_wanderMoveTimer <= 0f)
                {
                    _wanderState = WanderState.Idling;
                    _wanderIdleTimer = _stats.WanderIdleDuration;
                    _rb.linearVelocity = Vector2.zero;
                }
                break;

            case WanderState.Idling:
                _wanderIdleTimer -= Time.fixedDeltaTime;
                _rb.linearVelocity = Vector2.zero;

                if (_wanderIdleTimer <= 0f)
                {
                    PickNewWanderDirection();
                    _wanderState = WanderState.Moving;
                }
                break;
        }
    }

    protected virtual void PickNewWanderDirection()
    {
        _wanderDirection = Random.insideUnitCircle.normalized;
        _wanderMoveTimer = _stats.WanderMoveDuration;
    }
}