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

        [SerializeField] private WeaponType _weaponType = WeaponType.Invalid;

        private ItemDrops.WeaponItemData _weaponData;
        private IWeaponBehaviour _behaviour;
        private PlayerWeaponController _weaponController;
        private bool _isOnCooldown;
        private bool _isHoldingFire;

        public ItemDrops.WeaponItemData WeaponData => _weaponData;

        public void Initialize(ItemDrops.WeaponItemData weaponData)
        {
            _weaponData = weaponData;
        }

        protected virtual void Awake()
        {
            _weaponController = GetComponentInParent<PlayerWeaponController>();
            Assert.IsNotNull(_weaponController);

            Assert.IsTrue(_weaponType != WeaponType.Invalid);

            _behaviour = GetComponent<IWeaponBehaviour>();
            Assert.IsNotNull(_behaviour);
        }

        protected virtual void Start()
        {
            Assert.IsNotNull(_weaponData);
        }

        protected virtual void OnEnable()
        {
            switch (_weaponType)
            {
                case WeaponType.Primary:
                    _weaponController.PrimaryFireStarted += OnFireStarted;
                    _weaponController.PrimaryFireCanceled += OnFireCanceled;
                    break;
                case WeaponType.Secondary:
                    _weaponController.SecondaryFireStarted += OnFireStarted;
                    _weaponController.SecondaryFireCanceled += OnFireCanceled;
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
                    _weaponController.PrimaryFireStarted -= OnFireStarted;
                    _weaponController.PrimaryFireCanceled -= OnFireCanceled;
                    break;
                case WeaponType.Secondary:
                    _weaponController.SecondaryFireStarted -= OnFireStarted;
                    _weaponController.SecondaryFireCanceled -= OnFireCanceled;
                    break;
                case WeaponType.Invalid:
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void OnFireStarted(object obj, EventArgs args)
        {
            _isHoldingFire = true;
            StartCoroutine(HoldFireLoop());
        }

        private void OnFireCanceled(object obj, EventArgs args)
        {
            _isHoldingFire = false;
        }

        private IEnumerator HoldFireLoop()
        {
            while (_isHoldingFire)
            {
                yield return CooldownAndAttack();
            }
        }

        private IEnumerator CooldownAndAttack()
        {
            if (_isOnCooldown)
            {
                yield break;
            }

            yield return StartCoroutine(_behaviour.DoAttack());

            _isOnCooldown = true;
            yield return new WaitForSeconds(_weaponData.CooldownTime);
            _isOnCooldown = false;
        }
    }
}