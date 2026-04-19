using NUnit.Framework;
using UnityEngine;
using UnityEngine.Pool;

public class PooledEnemyProjectile : MonoBehaviour
{
    [SerializeField] private EnemyProjectileData _data;
    
    private IObjectPool<PooledEnemyProjectile> _myPool;
    private float _timer;
    private bool _isActive;
    private Vector2 _direction;
    
    private void Awake()
    {
        Assert.IsNotNull(_data);
    }
    public void Launch(Vector2 direction)
    {
        _direction = direction.normalized;
        
        float angle = Mathf.Atan2(_direction.y, _direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle + _data.ImageRotation);
        _timer = 0f;
        _isActive = true;
    }

    private void Update()
    {
        if (!_isActive) return;
        transform.Translate(_direction * (_data.Speed * Time.deltaTime), Space.World);
        
        _timer += Time.deltaTime;
        if (_timer >= _data.Lifetime)
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

    public void SetPool(IObjectPool<PooledEnemyProjectile> pool)
    {
        _myPool = pool;
        
    }

}
