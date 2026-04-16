using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField]
    private float _movementSpeed = 5f;
    [SerializeField]
    private float _detectionRadius = 5.0f;
    [SerializeField] 
    private float _chasingRadius = 20.0f;
    
    [Header("Audio Wrapper")]
    [SerializeField]
    private SoundEffect _detectionSound;

    [Header("References")] 
    [SerializeField]
    private Transform _playerTransform;
    
    [SerializeField]
    private Rigidbody2D _rb;
    [SerializeField]
    private AudioSource _audioSource;
    private bool _isChasing = false;
    
    void Update()
    {
        float distanceToPlayer = Vector2.Distance(_playerTransform.position, transform.position);

        if (IsPlayerInRange(distanceToPlayer))
        {
            _isChasing = true;
        }

        if (isPlayerOutsideChaseRadius(distanceToPlayer) && _isChasing)
        {
            _isChasing = false;
        }

        if (_isChasing)
        {
            ChasePlayer();
            if (!_detectionSound.IsPlaying(_audioSource))
            {
                _detectionSound.Play(_audioSource);
            }
        }
        else
        {
            _rb.linearVelocity = Vector2.zero;
        }

    }

    private void ChasePlayer()
    {
        Vector2 direction = ((Vector2)_playerTransform.position - (Vector2)transform.position).normalized;
        _rb.linearVelocity = direction *  _movementSpeed;
    }

    bool IsPlayerInRange(float distanceToPlayer)
    {
        return distanceToPlayer <= _detectionRadius;
    }
    
    bool isPlayerOutsideChaseRadius (float distanceToPlayer)
    {
        return distanceToPlayer >= _chasingRadius;
    }
}
