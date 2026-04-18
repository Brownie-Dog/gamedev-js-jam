using System.Collections.Generic;
using ItemDrops;
using UnityEngine;
using UnityEngine.Assertions;

namespace Player
{
    public class PlayerEquipment : MonoBehaviour
    {
        public const int MaxSlotCount = 5;

        public int MaxSlots => _hookPoints.Length;
        public int UnlockedSlots { get; private set; }

        [SerializeField]
        private int _initialUnlockedSlots = 1;

        [SerializeField]
        private Transform[] _hookPoints;

        [SerializeField]
        private GameObject[] _defaultWeapons;

        private GameObject[] _equippedWeaponInstances;
        private List<WeaponItemData> _equippedWeapons = new();

        private void Awake()
        {
            Assert.IsNotNull(_hookPoints);
            Assert.IsTrue(_hookPoints.Length <= MaxSlotCount);

            UnlockedSlots = _initialUnlockedSlots;

            _equippedWeaponInstances = new GameObject[MaxSlots];
        }

        private void Start()
        {
            SpawnDefaultWeapons();
        }

        public bool IsSlotUnlocked(int slotIndex)
        {
            return slotIndex >= 0 && slotIndex < UnlockedSlots;
        }

        public bool IsSlotEmpty(int slotIndex)
        {
            return slotIndex >= 0
                && slotIndex < UnlockedSlots
                && _equippedWeapons.Count > slotIndex
                && _equippedWeapons[slotIndex] != null;
        }

        public void UnlockSlot()
        {
            if (UnlockedSlots < MaxSlots)
            {
                UnlockedSlots++;
            }
        }

        public void Equip(WeaponItemData weapon, int slotIndex)
        {
            Assert.IsNotNull(weapon.WeaponPrefab);
            Assert.IsTrue(slotIndex >= 0 && slotIndex < UnlockedSlots);

            var oldInstance = _equippedWeaponInstances[slotIndex];
            if (oldInstance != null)
            {
                Destroy(oldInstance);
            }

            _equippedWeaponInstances[slotIndex] = Instantiate(
                weapon.WeaponPrefab,
                _hookPoints[slotIndex]
            );

            if (_equippedWeapons.Count <= slotIndex)
            {
                while (_equippedWeapons.Count <= slotIndex)
                {
                    _equippedWeapons.Add(null);
                }
            }

            _equippedWeapons[slotIndex] = weapon;
        }

        public void Unequip(int slotIndex)
        {
            Assert.IsTrue(slotIndex >= 0 && slotIndex < UnlockedSlots);

            var instance = _equippedWeaponInstances[slotIndex];
            if (instance != null)
            {
                Destroy(instance);
                _equippedWeaponInstances[slotIndex] = null;
            }

            if (slotIndex < _equippedWeapons.Count)
            {
                _equippedWeapons[slotIndex] = null;
            }
        }

        public bool HasWeapon(WeaponItemData weapon)
        {
            return _equippedWeapons.Contains(weapon);
        }

        public List<WeaponItemData> GetAllOwnedWeapons()
        {
            return _equippedWeapons;
        }

        private void SpawnDefaultWeapons()
        {
            for (int i = 0; i < _defaultWeapons.Length && i < UnlockedSlots; i++)
            {
                if (_defaultWeapons[i] != null)
                {
                    _equippedWeaponInstances[i] = Instantiate(_defaultWeapons[i], _hookPoints[i]);
                }
            }
        }
    }
}
