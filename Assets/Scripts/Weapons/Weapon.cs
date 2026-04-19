using System;
using System.Collections;
using Player;
using UnityEngine;
using UnityEngine.Assertions;

namespace Weapons
{
    public class Weapon : MonoBehaviour
    {
        public ItemDrops.WeaponItemData WeaponData { get; private set; }

        [Serializable]
        private enum WeaponType
        {
            Invalid,
            Primary,
            Secondary,
            None,
        }

        [SerializeField] private WeaponType _weaponType = WeaponType.Invalid;

        private IWeaponBehaviour _behaviour;
        private PlayerWeaponController _weaponController;
        private bool _isOnCooldown;
        private bool _isManualFiring;
        private bool _isAutoFiring;
        private Coroutine _holdFireCoroutine;

        private bool IsFiring => _isManualFiring || _isAutoFiring;


        public void Initialize(ItemDrops.WeaponItemData weaponData)
        {
            WeaponData = weaponData;
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
            Assert.IsNotNull(WeaponData);
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
                case WeaponType.None:
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
                case WeaponType.None:
                    break;
                case WeaponType.Invalid:
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void StartFiring()
        {
            _isAutoFiring = true;
            _holdFireCoroutine ??= StartCoroutine(HoldFireLoop());
        }

        public void StopFiring()
        {
            _isAutoFiring = false;
        }

        private void OnFireStarted(object obj, EventArgs args)
        {
            _isManualFiring = true;
            _holdFireCoroutine ??= StartCoroutine(HoldFireLoop());
        }

        private void OnFireCanceled(object obj, EventArgs args)
        {
            _isManualFiring = false;
        }

        private IEnumerator HoldFireLoop()
        {
            while (IsFiring)
            {
                yield return CooldownAndAttack();
            }

            _holdFireCoroutine = null;
        }

        private IEnumerator CooldownAndAttack()
        {
            if (_isOnCooldown)
            {
                yield break;
            }

            yield return StartCoroutine(_behaviour.DoAttack());

            _isOnCooldown = true;
            yield return new WaitForSeconds(WeaponData.CooldownTime);
            _isOnCooldown = false;
        }
    }
}