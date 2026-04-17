using System;
using UnityEngine;

public class EnemyDetection : MonoBehaviour
{
    [Header("Detection Settings")] 
    [SerializeField] private float _detectionRadius = 5.0f;
    [SerializeField] private float _chasingRadius = 20.0f;
    
    [Header("References")]
    [SerializeField] private Transform _player;
    [SerializeField] private SoundEffect _detectionSound;
    
    public event EventHandler OnPlayerDetected;
    public event EventHandler OnPlayerLost;

    private bool _isDetectingPlayer = false;

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
        return distanceToPlayer <= _detectionRadius;
    }
    
    
    private bool IsPlayerOutsideChaseRadius (float distanceToPlayer)
    {
        return distanceToPlayer >= _chasingRadius;
    }
}
