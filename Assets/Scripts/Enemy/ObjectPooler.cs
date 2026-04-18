using UnityEngine;
using UnityEngine.Pool;

public class ObjectPooler : MonoBehaviour
{
    [SerializeField] private Projectile _projectilePrefab;
    [SerializeField] private int _defaultCapacity = 15;
    [SerializeField] private int _maxSize = 30;
    
    private IObjectPool<Projectile> _pool;

    private void Awake()
    {
        _pool = new ObjectPool<Projectile>(
            CreateInstance,
            OnGet,
            OnRelease,
            OnDestroyInstance,
            false, _defaultCapacity, _maxSize);
    }
    private Projectile CreateInstance()
    {
        Projectile p = Instantiate(_projectilePrefab);
        p.SetPool(_pool);
        return p;
    }
    
    public Projectile Get() => _pool.Get();
    private void OnGet(Projectile p) => p.gameObject.SetActive(true);
    private void OnRelease(Projectile p) => p.gameObject.SetActive(false);
    private void OnDestroyInstance(Projectile p) => Destroy(p.gameObject);
}
