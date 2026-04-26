using System;
using UnityEngine;
using UnityEngine.Assertions;

public class EnemyDetection : MonoBehaviour
{
    [SerializeField] private EnemyStats _stats;

    private Transform _player;
    private bool _isDetectingPlayer = false;
    private float _detectionRadiusOffset = 0f;
    private float _chasingRadiusOffset = 0f;

    public event EventHandler OnPlayerDetected;
    public event EventHandler OnPlayerLost;

    private void Awake()
    {
        _player = GameObject.FindGameObjectWithTag("Player").transform;
        Assert.IsNotNull(_player);
    }

    public void IncreaseDetectionRange(float amount)
    {
        _detectionRadiusOffset += amount;
        _chasingRadiusOffset += amount;
    }

    private void Update()
    {
        float distanceToPlayer = Vector2.Distance(_player.position, transform.position);

        if (IsPlayerInRange(distanceToPlayer))
        {
            if (!_isDetectingPlayer)
            {
                _isDetectingPlayer = true;

                OnPlayerDetected?.Invoke(this, EventArgs.Empty);
            }
        }
        else if (IsPlayerOutsideChaseRadius(distanceToPlayer) && _isDetectingPlayer)
        {
            _isDetectingPlayer = false;

            OnPlayerLost?.Invoke(this, EventArgs.Empty);
        }
    }

    private bool IsPlayerInRange(float distanceToPlayer)
    {
        return distanceToPlayer <= _stats.DetectionRadius + _detectionRadiusOffset;
    }

    private bool IsPlayerOutsideChaseRadius(float distanceToPlayer)
    {
        return distanceToPlayer >= _stats.ChasingRadius + _chasingRadiusOffset;
    }
}