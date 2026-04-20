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
        public int MaxBaseSlots => _baseSlotCount;
        public int UnlockedBaseSlots { get; private set; }
        public int TotalSlots { get; private set; }

        [SerializeField]
        private int _initialUnlockedSlots = 1;

        [SerializeField]
        private WeaponItemData[] _defaultWeapons;

        [SerializeField]
        private PlayerInventory _inventory;

        private int _baseSlotCount;

        private readonly Dictionary<int, Transform> _hookPoints = new();
        private readonly Dictionary<int, SlotType> _slotTypes = new();
        private readonly Dictionary<int, WeaponItemData> _equippedWeapons = new();
        private readonly Dictionary<int, GameObject> _equippedWeaponInstances = new();
        private readonly Dictionary<int, List<int>> _extensionChildren = new();

        private int _nextExtensionSlotId;

        private void Awake()
        {
            Assert.IsNotNull(_inventory);

            var baseHookPoints = GetComponentsInChildren<HookPoint>();
            _baseSlotCount = baseHookPoints.Length;
            Assert.IsTrue(_initialUnlockedSlots <= _baseSlotCount);

            for (int i = 0; i < baseHookPoints.Length; i++)
            {
                _hookPoints[i] = baseHookPoints[i].transform;
                _slotTypes[i] = baseHookPoints[i].SlotType;
            }

            UnlockedBaseSlots = _initialUnlockedSlots;
            _nextExtensionSlotId = _baseSlotCount;
            TotalSlots = _initialUnlockedSlots;
        }

        private void Start()
        {
            SpawnDefaultWeapons();
        }

        private Transform GetHookPointForSlot(int slotId)
        {
            _hookPoints.TryGetValue(slotId, out var hookPoint);
            return hookPoint;
        }

        public SlotType GetSlotType(int slotId)
        {
            _slotTypes.TryGetValue(slotId, out var slotType);
            return slotType;
        }

        public bool IsSlotUnlocked(int slotId)
        {
            if (slotId >= 0 && slotId < _baseSlotCount)
            {
                return slotId < UnlockedBaseSlots;
            }

            return _hookPoints.ContainsKey(slotId);
        }

        public bool IsSlotEmpty(int slotId)
        {
            return IsSlotUnlocked(slotId) && !_equippedWeapons.ContainsKey(slotId);
        }

        public int FirstEmptySlot(WeaponItemData weapon)
        {
            var compatibleTypes = weapon.CompatibleSlotTypes;

            for (int i = 0; i < UnlockedBaseSlots; i++)
            {
                if (!_equippedWeapons.ContainsKey(i) && compatibleTypes.Contains(_slotTypes[i]))
                {
                    return i;
                }
            }

            foreach (var slotId in _hookPoints.Keys)
            {
                if (slotId < _baseSlotCount)
                {
                    continue;
                }

                if (!_equippedWeapons.ContainsKey(slotId) && compatibleTypes.Contains(_slotTypes[slotId]))
                {
                    return slotId;
                }
            }

            return -1;
        }

        public void UnlockSlot()
        {
            if (UnlockedBaseSlots < _baseSlotCount)
            {
                UnlockedBaseSlots++;
                TotalSlots++;
            }
        }

        public int UnlockExtensionSlot(int parentSlotId, Transform hookPoint, SlotType slotType)
        {
            int newSlotId = _nextExtensionSlotId++;
            TotalSlots++;

            _hookPoints[newSlotId] = hookPoint;
            _slotTypes[newSlotId] = slotType;

            if (!_extensionChildren.ContainsKey(parentSlotId))
            {
                _extensionChildren[parentSlotId] = new List<int>();
            }
            _extensionChildren[parentSlotId].Add(newSlotId);

            return newSlotId;
        }

        public void Equip(WeaponItemData weapon, int slotId)
        {
            Assert.IsNotNull(weapon.WeaponPrefab);
            Assert.IsTrue(IsSlotUnlocked(slotId));
            Assert.IsTrue(weapon.CompatibleSlotTypes.Contains(_slotTypes[slotId]));

            var hookPoint = GetHookPointForSlot(slotId);
            Assert.IsNotNull(hookPoint);

            _equippedWeapons.TryGetValue(slotId, out var existingWeapon);

            if (existingWeapon is ExtensionWeaponItemData)
            {
                CascadeToInventory(slotId);
            }

            if (_equippedWeaponInstances.TryGetValue(slotId, out var oldInstance) && oldInstance != null)
            {
                Destroy(oldInstance);
                _equippedWeaponInstances.Remove(slotId);
            }

            var instance = Instantiate(weapon.WeaponPrefab, hookPoint);
            var weaponComponent = instance.GetComponent<Weapon>();
            Assert.IsNotNull(weaponComponent);
            weaponComponent.Initialize(weapon);

            _equippedWeaponInstances[slotId] = instance;
            _equippedWeapons[slotId] = weapon;

            if (weapon is ExtensionWeaponItemData)
            {
                RegisterExtensionSlots(slotId, instance);
            }
        }

        private void RegisterExtensionSlots(int parentSlotId, GameObject instance)
        {
            foreach (var hookPoint in instance.GetComponentsInChildren<HookPoint>())
            {
                UnlockExtensionSlot(parentSlotId, hookPoint.transform, hookPoint.SlotType);
            }
        }

        public void Unequip(int slotId)
        {
            Assert.IsTrue(IsSlotUnlocked(slotId));

            _equippedWeapons.TryGetValue(slotId, out var existingWeapon);

            if (existingWeapon is ExtensionWeaponItemData)
            {
                CascadeToInventory(slotId);
            }

            if (_equippedWeaponInstances.TryGetValue(slotId, out var instance) && instance != null)
            {
                Destroy(instance);
                _equippedWeaponInstances.Remove(slotId);
            }

            _equippedWeapons.Remove(slotId);
        }

        private void CascadeToInventory(int slotId)
        {
            if (!_extensionChildren.TryGetValue(slotId, out var children))
            {
                return;
            }

            foreach (var childSlotId in children.ToList())
            {
                CascadeToInventory(childSlotId);

                if (_equippedWeapons.TryGetValue(childSlotId, out var childWeapon) && childWeapon != null)
                {
                    _inventory.AddItem(childWeapon);
                }

                if (_equippedWeaponInstances.TryGetValue(childSlotId, out var childInstance) && childInstance != null)
                {
                    Destroy(childInstance);
                }

                _equippedWeapons.Remove(childSlotId);
                _equippedWeaponInstances.Remove(childSlotId);
                _hookPoints.Remove(childSlotId);
                _slotTypes.Remove(childSlotId);
                TotalSlots--;
            }

            _extensionChildren.Remove(slotId);
        }

        public bool HasWeapon(WeaponItemData weapon)
        {
            return _equippedWeapons.ContainsValue(weapon);
        }

        public List<WeaponItemData> GetAllOwnedWeapons()
        {
            return _equippedWeapons.Values.Where(w => w != null).ToList();
        }

        private void SpawnDefaultWeapons()
        {
            foreach (var weapon in _defaultWeapons)
            {
                if (weapon == null)
                {
                    continue;
                }

                var slot = FirstEmptySlot(weapon);
                if (slot >= 0)
                {
                    Equip(weapon, slot);
                }
            }
        }
    }
}