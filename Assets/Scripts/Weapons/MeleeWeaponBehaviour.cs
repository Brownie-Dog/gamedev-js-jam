using System.Collections;
using Player;
using UnityEngine;
using UnityEngine.Assertions;

namespace Weapons
{
    [RequireComponent(typeof(DamageDealer))]
    public class MeleeWeaponBehaviour : MonoBehaviour, IWeaponBehaviour
    {
        [SerializeField] private ItemDrops.WeaponItemData _weaponData;
        private IWeaponAnimation _weaponAnimation;
        private DamageDealer _damageDealer;

        private void Awake()
        {
            _damageDealer = GetComponent<DamageDealer>();
            Assert.IsNotNull(_damageDealer);
            Assert.IsNotNull(_weaponData);

            _weaponAnimation = GetComponent<IWeaponAnimation>();
            Assert.IsNotNull(_weaponAnimation);
        }

        private void Start()
        {
            _damageDealer.Deactivate();
        }

        public IEnumerator DoAttack()
        {
            _damageDealer.Activate(_weaponData.Damage);

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