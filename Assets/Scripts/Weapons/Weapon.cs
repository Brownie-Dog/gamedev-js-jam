using System;
using System.Collections;
using Player;
using UnityEngine;
using UnityEngine.Assertions;

namespace Weapons
{
    public class Weapon : MonoBehaviour
    {
        [Serializable]
        private enum WeaponType
        {
            Invalid,
            Primary,
            Secondary
        }

        [SerializeField] protected ItemDrops.WeaponItemData _weaponData;

        [SerializeField] private WeaponType _weaponType = WeaponType.Invalid;

        private IWeaponBehaviour _behaviour;
        private PlayerWeaponController _weaponController;
        private bool _isOnCooldown;

        protected virtual void Awake()
        {
            _weaponController = GetComponentInParent<PlayerWeaponController>();
            Assert.IsNotNull(_weaponController);
            Assert.IsNotNull(_weaponData);

            Assert.IsTrue(_weaponType != WeaponType.Invalid);

            _behaviour = GetComponent<IWeaponBehaviour>();
            Assert.IsNotNull(_behaviour);
        }

        protected virtual void OnEnable()
        {
            switch (_weaponType)
            {
                case WeaponType.Primary:
                    _weaponController.PrimaryFireTriggered += OnAttack;
                    break;
                case WeaponType.Secondary:
                    _weaponController.SecondaryFireTriggered += OnAttack;
                    break;
                case WeaponType.Invalid:
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        protected virtual void OnDisable()
        {
            switch (_weaponType)
            {
                case WeaponType.Primary:
                    _weaponController.PrimaryFireTriggered -= OnAttack;
                    break;
                case WeaponType.Secondary:
                    _weaponController.SecondaryFireTriggered -= OnAttack;
                    break;
                case WeaponType.Invalid:
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void OnAttack(object obj, EventArgs args)
        {
            if (_isOnCooldown)
            {
                return;
            }

            StartCoroutine(CooldownAndAttack());
        }

        private IEnumerator CooldownAndAttack()
        {
            yield return StartCoroutine(_behaviour.DoAttack());

            _isOnCooldown = true;
            yield return new WaitForSeconds(_weaponData.CooldownTime);
            _isOnCooldown = false;
        }
    }
}