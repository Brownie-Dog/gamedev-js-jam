using System.Collections;
using Player;
using UnityEngine;
using UnityEngine.Assertions;

namespace Weapons
{
    [RequireComponent(typeof(DamageDealer))]
    public class SawbladeWeaponBehaviour : MonoBehaviour, IWeaponBehaviour
    {
        private Weapon _weapon;
        private IWeaponAnimation _weaponAnimation;
        private DamageDealer _damageDealer;

        private void Awake()
        {
            _weapon = GetComponent<Weapon>();
            Assert.IsNotNull(_weapon);

            _damageDealer = GetComponent<DamageDealer>();
            Assert.IsNotNull(_damageDealer);

            _weaponAnimation = GetComponent<IWeaponAnimation>();
            Assert.IsNotNull(_weaponAnimation);
        }

        private void Start()
        {
            var damageInfo = new DamageInfo(_weapon.WeaponData.Damage, Vector2.up * _weapon.WeaponData.KnockbackForce);
            _damageDealer.Activate(damageInfo);
        }

        public IEnumerator DoAttack()
        {
            try
            {
                yield return StartCoroutine(_weaponAnimation.PlayAnimation());
            }
            finally { }
        }
    }
}