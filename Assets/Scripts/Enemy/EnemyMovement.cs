using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField]
    private float _movementSpeed = 5f;
    [SerializeField]
    private float _detectionRadius = 5.0f;
    
    [Header("Audio Wrapper")]
    [SerializeField]
    private SoundEffect _detectionSound;

    [Header("References")] 
    [SerializeField]
    private Transform _playerTransform;
    
    private Rigidbody2D _rb;
    private AudioSource _audioSource;
    
    void Start() 
    {
        _rb = GetComponent<Rigidbody2D>();
        _playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        _audioSource = GetComponent<AudioSource>();
    }
    
    void Update()
    {
        float distanceToPlayer = Vector2.Distance(_playerTransform.position, transform.position);

        if (IsPlayerInRange(distanceToPlayer))
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
}
