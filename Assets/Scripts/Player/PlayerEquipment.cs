using System.Collections.Generic;
using System.Linq;
using ItemDrops;
using UnityEngine;
using UnityEngine.Assertions;
using Weapons;

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
        private WeaponItemData[] _defaultWeapons;

        private GameObject[] _equippedWeaponInstances;
        private List<WeaponItemData> _equippedWeapons = new();

        private void Awake()
        {
            Assert.IsNotNull(_hookPoints);
            Assert.IsTrue(_hookPoints.Length <= MaxSlotCount);

            UnlockedSlots = _initialUnlockedSlots;

            _equippedWeaponInstances = new GameObject[MaxSlots];
            for (int i = 0; i < UnlockedSlots; i++)
            {
                _equippedWeapons.Add(null);
            }
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
                && _equippedWeapons[slotIndex] == null;
        }

        public int FirstEmptySlot()
        {
            for (int i = 0; i < UnlockedSlots; i++)
            {
                if (_equippedWeapons[i] == null)
                {
                    return i;
                }
            }
            return -1;
        }

        public void UnlockSlot()
        {
            if (UnlockedSlots < MaxSlots)
            {
                UnlockedSlots++;
                _equippedWeapons.Add(null);
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

            var instance = Instantiate(
                weapon.WeaponPrefab,
                _hookPoints[slotIndex]
            );

            var weaponComponent = instance.GetComponent<Weapon>();
            Assert.IsNotNull(weaponComponent);
            weaponComponent.Initialize(weapon);

            _equippedWeaponInstances[slotIndex] = instance;
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

            _equippedWeapons[slotIndex] = null;
        }

        public bool HasWeapon(WeaponItemData weapon)
        {
            return _equippedWeapons.Contains(weapon);
        }

        public List<WeaponItemData> GetAllOwnedWeapons()
        {
            return _equippedWeapons.Where(w => w != null).ToList();
        }

        private void SpawnDefaultWeapons()
        {
            for (int i = 0; i < _defaultWeapons.Length && i < UnlockedSlots; i++)
            {
                if (_defaultWeapons[i] != null)
                {
                    Equip(_defaultWeapons[i], i);
                }
            }
        }
    }
}
