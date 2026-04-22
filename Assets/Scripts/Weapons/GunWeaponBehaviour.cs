using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;
using Weapons;

public class GunWeaponBehaviour : MonoBehaviour, IWeaponBehaviour
{
    [SerializeField] private Bullet _bulletPrefab;
    [SerializeField] private int _poolSize = 50;
    [SerializeField] private Transform _firePoint;
    [SerializeField] private int _penetrationCount = 1;

    private Weapon _weapon;
    private MonoBehaviourPool<Bullet> _bulletPool;

    private void Awake()
    {
        _weapon = GetComponent<Weapon>();
        Assert.IsNotNull(_weapon);

        Assert.IsNotNull(_bulletPrefab);
        Assert.IsNotNull(_firePoint);

        _bulletPool = new MonoBehaviourPool<Bullet>(_bulletPrefab, _poolSize, _poolSize, transform,
            b => b.SetPool(_bulletPool.Pool)
        );
    }

    public IEnumerator DoAttack()
    {
        FireBullet();
        yield break;
    }

    private void FireBullet()
    {
        var damageInfo = new DamageInfo(_weapon.WeaponData.Damage, Vector2.up * _weapon.WeaponData.KnockbackForce);
        Bullet bullet = _bulletPool.Get();
        bullet.Activate(_firePoint.position, _firePoint.up, damageInfo, _penetrationCount);
    }
}