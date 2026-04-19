using System.Collections;
using Player;
using UnityEngine;
using UnityEngine.Assertions;

namespace Weapons
{
    [RequireComponent(typeof(DamageDealer))]
    public class MeleeWeaponBehaviour : MonoBehaviour, IWeaponBehaviour
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
            _damageDealer.Deactivate();
        }

        public IEnumerator DoAttack()
        {
            _damageDealer.Activate(_weapon.WeaponData.Damage);

            try
            {
                yield return StartCoroutine(_weaponAnimation.PlayAnimation());
            }
            finally
            {
                _damageDealer.Deactivate();
            }
        }
    }
}