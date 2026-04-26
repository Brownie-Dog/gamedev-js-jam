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
        [SerializeField] private PlayerEquipment _playerEquipment;

        [SerializeField] private PlayerInventory _playerInventory;

        [SerializeField] private Transform _slotContainer;

        [SerializeField] private Transform _inventoryContainer;

        [SerializeField] private LoadoutSlotUI _slotUIPrefab;

        [SerializeField] private LoadoutInventoryItemUI _inventoryItemPrefab;

        [SerializeField] private GameObject _canvasRoot;

        [SerializeField] private Camera _mainCamera;

        [SerializeField] private SoundEffect _dropSound;

        [SerializeField] private Color _emptyGeneralSlotColor = new Color(0.04f, 0.94f, 0.93f, 0.5f);

        [SerializeField] private Color _occupiedGeneralSlotColor = new Color(0.8f, 0.2f, 0.2f, 0.5f);

        [SerializeField] private Color _emptyBackSlotColor = new Color(0.53f, 0.34f, 0.85f, 0.5f);

        [SerializeField] private Color _occupiedBackSlotColor = new Color(0.8f, 0.2f, 0.2f, 0.5f);

        private WeaponItemData CurrentlyDraggedWeapon { get; set; }
        private readonly Dictionary<int, LoadoutSlotUI> _slotUIs = new();
        private readonly List<LoadoutInventoryItemUI> _inventoryItemUIs = new();

        private void Awake()
        {
            Assert.IsNotNull(_playerEquipment);
            Assert.IsNotNull(_playerInventory);
            Assert.IsNotNull(_slotContainer);
            Assert.IsNotNull(_inventoryContainer);
            Assert.IsNotNull(_slotUIPrefab);
            Assert.IsNotNull(_inventoryItemPrefab);
            Assert.IsNotNull(_canvasRoot);
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
            _canvasRoot.SetActive(true);
            RefreshAll();
        }

        public void Hide()
        {
            _canvasRoot.SetActive(false);
            ClearAll();
        }

        public void SetDraggingWeapon(WeaponItemData weapon)
        {
            CurrentlyDraggedWeapon = weapon;
            UpdateSlotHighlights();
        }

        public void ClearDraggingWeapon()
        {
            CurrentlyDraggedWeapon = null;
            UpdateSlotHighlights();
        }

        private void UpdateSlotHighlights()
        {
            if (CurrentlyDraggedWeapon == null)
            {
                foreach (var slotUI in _slotUIs.Values)
                {
                    slotUI.ShowCompatibleItemSlot(isCompatible: false, isDragging: false);
                }

                return;
            }

            var compatibleTypes = CurrentlyDraggedWeapon.CompatibleSlotTypes;

            foreach (var slotData in _slotUIs)
            {
                var slotUI = slotData.Value;
                var slotType = _playerEquipment.GetSlotType(slotData.Key);
                var isCompatible = compatibleTypes.Contains(slotType);
                slotUI.ShowCompatibleItemSlot(isCompatible: isCompatible, isDragging: true);
            }
        }

        public void HandleDropOnSlot(WeaponItemData weapon, bool dragFromEquipment, int sourceSlotId,
            int sourceInventoryIndex, int targetSlotId)
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
                _dropSound?.Play();
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
            _dropSound?.Play();
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
            _dropSound?.Play();
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

                var slotType = _playerEquipment.GetSlotType(slotId);
                var emptySlotColor = slotType == SlotType.Back ? _emptyBackSlotColor : _emptyGeneralSlotColor;
                var occupiedSlotColor = slotType == SlotType.Back ? _occupiedBackSlotColor : _occupiedGeneralSlotColor;
                slotUI.Initialize(slotId, weapon, emptySlotColor, occupiedSlotColor, slotType);
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