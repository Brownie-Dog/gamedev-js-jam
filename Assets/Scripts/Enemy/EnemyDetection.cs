using System;
using UnityEngine;
using UnityEngine.Assertions;

public class EnemyDetection : MonoBehaviour
{
    [Header("References")]
    private Transform _player;
    [SerializeField] private SoundEffect _detectionSound;
    [SerializeField] private EnemyStats _stats;
    
    public event EventHandler OnPlayerDetected;
    public event EventHandler OnPlayerLost;

    private bool _isDetectingPlayer = false;

    
    private void Awake()
    {
        _player = GameObject.FindGameObjectWithTag("Player").transform;
        Assert.IsNotNull(_player);
    }
    
    private void Update()
    {
        if (!_player) return;

        float distanceToPlayer = Vector2.Distance(_player.position, transform.position);

        if (IsPlayerInRange(distanceToPlayer))
        {
            if (!_isDetectingPlayer)
            {
                _isDetectingPlayer = true;
                if (_detectionSound)
                {
                    _detectionSound.Play();
                }

                OnPlayerDetected?.Invoke(this, EventArgs.Empty);
            }
        }
        else if (IsPlayerOutsideChaseRadius(distanceToPlayer) && _isDetectingPlayer)
        {
            _isDetectingPlayer = false;
            if (_detectionSound)
            {
                _detectionSound.Stop();
            }
            OnPlayerLost?.Invoke(this, EventArgs.Empty);
        }
    }
    
    private bool IsPlayerInRange(float distanceToPlayer)
    {
        return distanceToPlayer <= _stats.detectionRadius;
    }
    
    
    private bool IsPlayerOutsideChaseRadius (float distanceToPlayer)
    {
        return distanceToPlayer >= _stats.chasingRadius;
    }
}
