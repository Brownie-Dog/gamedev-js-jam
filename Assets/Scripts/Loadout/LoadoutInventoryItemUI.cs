using ItemDrops;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Loadout
{
    public class LoadoutInventoryItemUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [SerializeField] private Image _iconImage;

        private LoadoutDragHandler _dragHandler;

        private WeaponItemData _weapon;
        private int _inventoryIndex;

        private void Awake()
        {
            _dragHandler = GetComponentInParent<LoadoutDragHandler>();
            Assert.IsNotNull(_dragHandler);
        }

        public void Initialize(WeaponItemData weapon, int inventoryIndex)
        {
            _weapon = weapon;
            _inventoryIndex = inventoryIndex;

            _iconImage.sprite = weapon.Icon;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            _dragHandler.StartDragFromInventory(_weapon, _inventoryIndex, eventData.position);
        }

        public void OnDrag(PointerEventData eventData)
        {
            _dragHandler.OnDrag(eventData);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            _dragHandler.OnEndDrag(eventData);
        }
    }
}