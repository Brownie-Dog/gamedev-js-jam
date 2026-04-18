using System;
using Player;
using UnityEngine;
using UnityEngine.Assertions;

namespace Weapons
{
    public abstract class SecondaryWeapon : MonoBehaviour
    {
        private PlayerWeaponController _weaponController;

        protected virtual void Awake()
        {
            _weaponController = GetComponentInParent<PlayerWeaponController>();
            Assert.IsNotNull(_weaponController);
        }

        protected virtual void OnEnable()
        {
            _weaponController.SecondaryFireTriggered += OnSecondaryAttack;
        }

        protected virtual void OnDisable()
        {
            _weaponController.SecondaryFireTriggered -= OnSecondaryAttack;
        }

        protected abstract void OnSecondaryAttack(object obj, EventArgs args);
    }
}
