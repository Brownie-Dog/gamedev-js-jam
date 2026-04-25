using ItemDrops;
using Player;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Loadout
{
    public class LoadoutDragHandler : MonoBehaviour, IDragHandler, IEndDragHandler
    {
        public bool IsDragging { get; private set; }
        public WeaponItemData DraggedWeapon { get; private set; }

        [SerializeField] private Image _dragIcon;

        [SerializeField] private LoadoutUI _loadoutUI;

        [SerializeField] private PlayerEquipment _playerEquipment;

        [SerializeField] private PlayerInventory _playerInventory;

        private void Awake()
        {
            Assert.IsNotNull(_dragIcon);
            Assert.IsNotNull(_loadoutUI);
            Assert.IsNotNull(_playerEquipment);
            Assert.IsNotNull(_playerInventory);
        }

        private int _sourceSlotId = -1;
        private int _sourceInventoryIndex = -1;
        private bool _dragFromEquipment;
        private Vector2 _lastPointerPosition;

        public void StartDragFromInventory(WeaponItemData weapon, int inventoryIndex, Vector2 pointerPosition)
        {
            DraggedWeapon = weapon;
            _sourceInventoryIndex = inventoryIndex;
            _sourceSlotId = -1;
            _dragFromEquipment = false;
            _lastPointerPosition = pointerPosition;
            ShowDragIcon(weapon);
            _loadoutUI.SetDraggingWeapon(weapon);
        }

        public void StartDragFromSlot(WeaponItemData weapon, int slotId, Vector2 pointerPosition)
        {
            DraggedWeapon = weapon;
            _sourceSlotId = slotId;
            _sourceInventoryIndex = -1;
            _dragFromEquipment = true;
            _lastPointerPosition = pointerPosition;
            ShowDragIcon(weapon);
            _loadoutUI.SetDraggingWeapon(weapon);
        }

        private void ShowDragIcon(WeaponItemData weapon)
        {
            IsDragging = true;
            _dragIcon.sprite = weapon.Icon;
            _dragIcon.enabled = true;
            _dragIcon.transform.position = _lastPointerPosition;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!IsDragging)
            {
                return;
            }

            _lastPointerPosition = eventData.position;
            _dragIcon.transform.position = eventData.position;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (!IsDragging)
            {
                return;
            }

            IsDragging = false;
            _loadoutUI.ClearDraggingWeapon();
            _dragIcon.enabled = false;

            var dropTarget = eventData.pointerCurrentRaycast.gameObject;

            if (dropTarget != null)
            {
                var slotUI = dropTarget.GetComponentInParent<LoadoutSlotUI>();
                if (slotUI != null)
                {
                    _loadoutUI.HandleDropOnSlot(DraggedWeapon, _dragFromEquipment, _sourceSlotId, _sourceInventoryIndex,
                        slotUI.SlotId
                    );
                    return;
                }
            }

            _loadoutUI.HandleDropOnInventory(DraggedWeapon, _dragFromEquipment, _sourceSlotId);
        }
    }
}