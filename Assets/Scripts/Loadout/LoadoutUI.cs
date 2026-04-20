using System;
using System.Collections.Generic;
using System.Linq;
using ItemDrops;
using Player;
using UnityEngine;
using UnityEngine.Assertions;
using Weapons;

namespace Loadout
{
    public class LoadoutUI : MonoBehaviour
    {
        [SerializeField]
        private PlayerEquipment _playerEquipment;

        [SerializeField]
        private PlayerInventory _playerInventory;

        [SerializeField]
        private Transform _slotContainer;

        [SerializeField]
        private Transform _inventoryContainer;

        [SerializeField]
        private LoadoutSlotUI _slotUIPrefab;

        [SerializeField]
        private LoadoutInventoryItemUI _inventoryItemPrefab;

        [SerializeField]
        private GameObject _canvasRoot;

        [SerializeField]
        private Camera _mainCamera;

[SerializeField]
        private Color _emptySlotColor = new Color(0.2f, 0.8f, 0.2f, 0.5f);

        [SerializeField]
        private Color _occupiedSlotColor = new Color(0.8f, 0.2f, 0.2f, 0.7f);

        private readonly Dictionary<int, LoadoutSlotUI> _slotUIs = new();
        private readonly List<LoadoutInventoryItemUI> _inventoryItemUIs = new();

        private void Awake()
        {
            Assert.IsNotNull(_mainCamera);
            _canvasRoot.SetActive(false);
        }

        private void OnEnable()
        {
            _playerEquipment.OnItemEquipped += HandleEquipmentChanged;
            _playerEquipment.OnItemUnequipped += HandleEquipmentChanged;
            _playerInventory.OnInventoryChanged += HandleInventoryChanged;
        }

        private void OnDisable()
        {
            _playerEquipment.OnItemEquipped -= HandleEquipmentChanged;
            _playerEquipment.OnItemUnequipped -= HandleEquipmentChanged;
            _playerInventory.OnInventoryChanged -= HandleInventoryChanged;
        }

        private void LateUpdate()
        {
            if (!_canvasRoot.activeSelf)
            {
                return;
            }

            PositionSlotsOverHookPoints();
        }

        public void Show()
        {
            if (_canvasRoot == null)
            {
                return;
            }

            _canvasRoot.SetActive(true);
            RefreshAll();
        }

        public void Hide()
        {
            if (_canvasRoot == null)
            {
                return;
            }

            _canvasRoot.SetActive(false);
            ClearAll();
        }

        public void HandleDropOnSlot(WeaponItemData weapon, bool dragFromEquipment, int sourceSlotId, int sourceInventoryIndex, int targetSlotId)
        {
            var targetSlotType = _playerEquipment.GetSlotType(targetSlotId);

            if (!weapon.CompatibleSlotTypes.Contains(targetSlotType))
            {
                return;
            }

            if (dragFromEquipment)
            {
                HandleEquipmentToSlot(weapon, sourceSlotId, targetSlotId);
            }
            else
            {
                HandleInventoryToSlot(weapon, sourceInventoryIndex, targetSlotId);
            }
        }

        public void HandleDropOnInventory(WeaponItemData weapon, bool dragFromEquipment, int sourceSlotId)
        {
            if (!dragFromEquipment)
            {
                return;
            }

            var unequipped = _playerEquipment.UnequipWithReturn(sourceSlotId);
            if (unequipped != null)
            {
                _playerInventory.AddItem(unequipped);
            }
        }

        private void HandleEquipmentToSlot(WeaponItemData weapon, int sourceSlotId, int targetSlotId)
        {
            if (sourceSlotId == targetSlotId)
            {
                return;
            }

            var existingInTarget = _playerEquipment.GetWeaponInSlot(targetSlotId);
            var sourceSlotType = _playerEquipment.GetSlotType(sourceSlotId);

            _playerEquipment.UnequipWithReturn(sourceSlotId);

            if (existingInTarget != null)
            {
                var unequippedTarget = _playerEquipment.UnequipWithReturn(targetSlotId);

                if (unequippedTarget.CompatibleSlotTypes.Contains(sourceSlotType))
                {
                    _playerEquipment.TryEquip(unequippedTarget, sourceSlotId);
                }
                else
                {
                    _playerInventory.AddItem(unequippedTarget);
                }
            }

            _playerEquipment.TryEquip(weapon, targetSlotId);
        }

        private void HandleInventoryToSlot(WeaponItemData weapon, int sourceInventoryIndex, int targetSlotId)
        {
            if (sourceInventoryIndex < 0 || sourceInventoryIndex >= _playerInventory.Items.Count)
            {
                return;
            }

            if (_playerInventory.Items[sourceInventoryIndex] != weapon)
            {
                return;
            }

            var existingInTarget = _playerEquipment.GetWeaponInSlot(targetSlotId);
            if (existingInTarget != null)
            {
                _playerEquipment.UnequipWithReturn(targetSlotId);
                _playerInventory.AddItem(existingInTarget);
            }

            _playerInventory.RemoveItem(weapon);
            _playerEquipment.TryEquip(weapon, targetSlotId);
        }

        private void PositionSlotsOverHookPoints()
        {
            foreach (var kvp in _slotUIs)
            {
                var slotId = kvp.Key;
                var slotUI = kvp.Value;
                var hookPoint = _playerEquipment.GetHookPoint(slotId);

                if (hookPoint == null)
                {
                    continue;
                }

                var screenPos = _mainCamera.WorldToScreenPoint(hookPoint.position);
                slotUI.transform.position = screenPos;
            }
        }

        private void RefreshAll()
        {
            RefreshSlots();
            RefreshInventory();
            PositionSlotsOverHookPoints();
        }

        private void RefreshSlots()
        {
            ClearSlots();

            foreach (var slotId in _playerEquipment.GetAllSlotIds())
            {
                var slotUI = Instantiate(_slotUIPrefab, _slotContainer);
                var weapon = _playerEquipment.GetWeaponInSlot(slotId);

                slotUI.Initialize(slotId, weapon, _emptySlotColor, _occupiedSlotColor);
                _slotUIs[slotId] = slotUI;
            }
        }

        private void RefreshInventory()
        {
            ClearInventory();

            for (int i = 0; i < _playerInventory.Items.Count; i++)
            {
                var item = _playerInventory.Items[i];
                if (item is not WeaponItemData weapon)
                {
                    continue;
                }

                var itemUI = Instantiate(_inventoryItemPrefab, _inventoryContainer);
                itemUI.Initialize(weapon, i);
                _inventoryItemUIs.Add(itemUI);
            }
        }

        private void ClearSlots()
        {
            foreach (var slotUI in _slotUIs.Values)
            {
                Destroy(slotUI.gameObject);
            }
            _slotUIs.Clear();
        }

        private void ClearInventory()
        {
            foreach (var itemUI in _inventoryItemUIs)
            {
                Destroy(itemUI.gameObject);
            }
            _inventoryItemUIs.Clear();
        }

        private void ClearAll()
        {
            ClearSlots();
            ClearInventory();
        }

        private void HandleEquipmentChanged(int slotId, WeaponItemData weapon)
        {
            RefreshAll();
        }

        private void HandleInventoryChanged()
        {
            RefreshAll();
        }
    }
}