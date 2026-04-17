using System;
using UnityEngine;
using UnityEngine.Assertions;

namespace Weapons
{
    public abstract class PrimaryWeapon : MonoBehaviour
    {
        private PlayerWeaponController _weaponController;

        protected virtual void Awake()
        {
            _weaponController = GetComponentInParent<PlayerWeaponController>();
            Assert.IsNotNull(_weaponController);
        }

        protected virtual void OnEnable()
        {
            _weaponController.PrimaryFireTriggered += OnPrimaryAttack;
        }

        protected virtual void OnDisable()
        {
            _weaponController.PrimaryFireTriggered -= OnPrimaryAttack;
        }

        protected abstract void OnPrimaryAttack(object obj, EventArgs args);
    }
}
