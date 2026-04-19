using System.Collections;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.Assertions;

namespace Weapons
{
    public class GunWeaponBehaviour : MonoBehaviour, IWeaponBehaviour
    {
        [SerializeField] private Bullet _bulletPrefab;
        [SerializeField] private int _poolSize = 50;
        [SerializeField] private Transform _firePoint;

        private Weapon _weapon;
        private IObjectPool<Bullet> _bulletPool;

        private void Awake()
        {
            _weapon = GetComponent<Weapon>();
            Assert.IsNotNull(_weapon);

            Assert.IsNotNull(_bulletPrefab);
            Assert.IsNotNull(_firePoint);

            _bulletPool = new ObjectPool<Bullet>(
                CreateBullet,
                OnGetBullet,
                OnReleaseBullet,
                OnDestroyBullet,
                defaultCapacity: _poolSize,
                maxSize: _poolSize
            );
        }

        private Bullet CreateBullet()
        {
            var bullet = Instantiate(_bulletPrefab, transform);
            bullet.SetPool(_bulletPool);
            return bullet;
        }

        private void OnGetBullet(Bullet bullet)
        {
            bullet.gameObject.SetActive(true);
        }

        private void OnReleaseBullet(Bullet bullet)
        {
            bullet.Deactivate();
            bullet.gameObject.SetActive(false);
        }

        private void OnDestroyBullet(Bullet bullet)
        {
            Destroy(bullet.gameObject);
        }

        public IEnumerator DoAttack()
        {
            FireBullet();
            yield break;
        }

        private void FireBullet()
        {
            var bullet = _bulletPool.Get();
            bullet.Activate(_firePoint.position, transform.up, _weapon.WeaponData.Damage);
        }
    }
}