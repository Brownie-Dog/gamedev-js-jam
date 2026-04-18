using UnityEngine;
using UnityEngine.Pool;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float _speed = 10f;
    private Vector2 _direction;
    [SerializeField] private float _imageRotation = 180f;
    [SerializeField] private float _lifetime = 5f;
    
    private IObjectPool<Projectile> _myPool;
    public void SetPool(IObjectPool<Projectile> pool) => _myPool = pool;
    private float _timer;
    private bool _isActive;

    public void Launch(Vector2 direction)
    {
        _direction = direction.normalized;
        
        float angle = Mathf.Atan2(_direction.y, _direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle + _imageRotation);
        _timer = 0f;
        _isActive = true;
    }

    private void Update()
    {
        if (!_isActive) return;
        transform.Translate(_direction * (_speed * Time.deltaTime), Space.World);
        
        _timer += Time.deltaTime;
        if (_timer >= _lifetime)
        {
            ReturnToPool();
        }
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // DoDamage()
            Debug.Log("You can't even dodge the bullet?");
            ReturnToPool();
        }
    }
    
    private void ReturnToPool()
    {
        _myPool.Release(this);
    }
}
