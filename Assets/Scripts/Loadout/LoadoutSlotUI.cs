using ItemDrops;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Weapons;

namespace Loadout
{
    public class LoadoutSlotUI : MonoBehaviour, IDropHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        public RectTransform RectTransform { get; private set; }

        [SerializeField] private Image _backgroundImage;

        [SerializeField] private Image _iconImage;

        private LoadoutDragHandler _dragHandler;

        public int SlotId { get; private set; }

        private Color _emptyColor;
        private Color _occupiedColor;
        private WeaponItemData _weapon;

        private void Awake()
        {
            RectTransform = (RectTransform)transform;
            
            _dragHandler = GetComponentInParent<LoadoutDragHandler>();
            Assert.IsNotNull(_dragHandler);
            
            Assert.IsNotNull(_backgroundImage);
            Assert.IsNotNull(_iconImage);
        }

        public void Initialize(int slotId, WeaponItemData weapon, Color emptyColor, Color occupiedColor)
        {
            SlotId = slotId;
            _emptyColor = emptyColor;
            _occupiedColor = occupiedColor;
            Refresh(weapon);
        }

        public void Refresh(WeaponItemData weapon)
        {
            _weapon = weapon;

            if (weapon != null)
            {
                _iconImage.sprite = weapon.Icon;
                _iconImage.enabled = true;
                _backgroundImage.color = _occupiedColor;
            }
            else
            {
                _iconImage.sprite = null;
                _iconImage.enabled = false;
                _backgroundImage.color = _emptyColor;
            }
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (_weapon != null)
            {
                _dragHandler.StartDragFromSlot(_weapon, SlotId, eventData.position);
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            _dragHandler.OnDrag(eventData);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            _dragHandler.OnEndDrag(eventData);
        }

        public void OnDrop(PointerEventData eventData)
        {
        }
    }
}