using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;
using Weapons;

public class PlayerWeaponController : MonoBehaviour
{
    public EventHandler PrimaryFireTriggered;
    public EventHandler SecondaryFireTriggered;

    [Serializable]
    private class WeaponSlot
    {
        [CanBeNull]
        public GameObject WeaponPrefab;
        public Transform HookPoint;
    }

    [SerializeField]
    private List<WeaponSlot> _weaponSlots;

    private void Awake()
    {
        foreach (WeaponSlot slot in _weaponSlots)
        {
            Assert.IsNotNull(slot.HookPoint);
        }
    }

    // TODO: Replace this with weapon pickups
    private void Start()
    {
        SpawnWeaponsAtSlots();
    }

    // TODO: Replace this with weapon pickups
    private void SpawnWeaponsAtSlots()
    {
        foreach (WeaponSlot weapon in _weaponSlots)
        {
            if (weapon.WeaponPrefab != null)
            {
                Instantiate(weapon.WeaponPrefab, weapon.HookPoint);
            }
        }
    }

    public void InputEvent_OnPrimaryFire(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            PrimaryFireTriggered?.Invoke(this, EventArgs.Empty);
        }
    }

    public void InputEvent_OnSecondaryFire(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            SecondaryFireTriggered?.Invoke(this, EventArgs.Empty);
        }
    }
}
