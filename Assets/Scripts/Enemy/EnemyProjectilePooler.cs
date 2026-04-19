using UnityEngine;
using UnityEngine.Pool;

public class EnemyProjectilePooler : MonoBehaviour
{
    [SerializeField] private PooledEnemyProjectile _projectilePrefab;
    [SerializeField] private EnemyProjectileData _data;
    
    private IObjectPool<PooledEnemyProjectile> _pool;

    private void Awake()
    {
        _pool = new ObjectPool<PooledEnemyProjectile>(
            createFunc: CreateInstance,
            actionOnGet: (p) => p.gameObject.SetActive(true),
            actionOnRelease: (p) => p.gameObject.SetActive(false),
            actionOnDestroy: (p) => Destroy(p.gameObject),
            collectionCheck: false, 
            defaultCapacity: _data.PoolDefaultCapacity, 
            maxSize: _data.PoolMaxSize
        );
    }
    private PooledEnemyProjectile CreateInstance()
    {
        PooledEnemyProjectile p = Instantiate(_projectilePrefab);
        p.SetPool(_pool);
        return p;
    }
    
    public PooledEnemyProjectile RequestProjectile() => _pool.Get();

}
