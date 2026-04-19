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
        private FireMode _fireMode;
        private bool _isHoldingFire;

        public FireMode FireMode
        {
            get => _fireMode;
            set => _fireMode = value;
        }

        protected virtual void Awake()
        {
            _weaponController = GetComponentInParent<PlayerWeaponController>();
            Assert.IsNotNull(_weaponController);
            Assert.IsNotNull(_weaponData);

            Assert.IsTrue(_weaponType != WeaponType.Invalid);

            _behaviour = GetComponent<IWeaponBehaviour>();
            Assert.IsNotNull(_behaviour);

            _fireMode = _weaponData.DefaultFireMode;
        }

        protected virtual void OnEnable()
        {
            switch (_weaponType)
            {
                case WeaponType.Primary:
                    _weaponController.PrimaryFireTriggered += OnAttack;
                    _weaponController.PrimaryFireStarted += OnFireStarted;
                    _weaponController.PrimaryFireCanceled += OnFireCanceled;
                    break;
                case WeaponType.Secondary:
                    _weaponController.SecondaryFireTriggered += OnAttack;
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
                    _weaponController.PrimaryFireTriggered -= OnAttack;
                    _weaponController.PrimaryFireStarted -= OnFireStarted;
                    _weaponController.PrimaryFireCanceled -= OnFireCanceled;
                    break;
                case WeaponType.Secondary:
                    _weaponController.SecondaryFireTriggered -= OnAttack;
                    _weaponController.SecondaryFireStarted -= OnFireStarted;
                    _weaponController.SecondaryFireCanceled -= OnFireCanceled;
                    break;
                case WeaponType.Invalid:
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void OnAttack(object obj, EventArgs args)
        {
            if (_fireMode == FireMode.Tap)
            {
                TryAttack();
            }
        }

        private void OnFireStarted(object obj, EventArgs args)
        {
            if (_fireMode == FireMode.Hold)
            {
                _isHoldingFire = true;
                StartCoroutine(HoldFireLoop());
            }
        }

        private void OnFireCanceled(object obj, EventArgs args)
        {
            if (_fireMode == FireMode.Hold)
            {
                _isHoldingFire = false;
            }
        }

        private IEnumerator HoldFireLoop()
        {
            while (_isHoldingFire)
            {
                yield return TryAttackCoroutine();
            }
        }

        private void TryAttack()
        {
            if (_isOnCooldown)
            {
                return;
            }

            StartCoroutine(CooldownAndAttack());
        }

        private IEnumerator TryAttackCoroutine()
        {
            if (_isOnCooldown)
            {
                yield break;
            }

            yield return CooldownAndAttack();
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