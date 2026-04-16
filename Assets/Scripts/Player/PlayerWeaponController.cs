using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using Weapons;

public class PlayerWeaponController : MonoBehaviour
{
    [Serializable]
    private class WeaponSlot
    {
        public GameObject WeaponPrefab;
        public Transform HookPoint;

        public void Initialize()
        {
            Assert.IsNotNull(WeaponPrefab);
            Assert.IsNotNull(HookPoint);
        }
    }

    [SerializeField] private List<WeaponSlot> _weaponSlots;

    private void Awake()
    {
        foreach (WeaponSlot slot in _weaponSlots)
        {
            slot.Initialize();
        }
    }

    private void Start()
    {
        SpawnWeaponsAtSlots();
    }

    private void SpawnWeaponsAtSlots()
    {
        foreach (WeaponSlot weapon in _weaponSlots)
        {
            Instantiate(weapon.WeaponPrefab, weapon.HookPoint);
        }
    }
}