using System.Linq;
using ItemDrops;
using UnityEngine;
using UnityEngine.Assertions;

namespace Shop
{
    public class ShopItem : MonoBehaviour, IInteractable
    {
        public bool Sold { get; private set; }
        public bool IsPlayerInRange { get; private set; }

        [SerializeField] private SpriteRenderer _weaponSpriteRenderer;
        [SerializeField] private Material _highlightMaterial;
        [SerializeField] private Transform _infoPanel;
        [SerializeField] private TMPro.TextMeshPro _nameText;
        [SerializeField] private TMPro.TextMeshPro _priceText;
        [SerializeField] private Collider2D _interactionCollider;
        [SerializeField] private float _infoPanelYOffset = 0.5f;

        private ShopContext _context;
        private ShopEntry _currentEntry;
        private Material _defaultMaterial;
        private bool _isHighlighted;

        private void Awake()
        {
            Assert.IsNotNull(_weaponSpriteRenderer);
            Assert.IsNotNull(_highlightMaterial);
            Assert.IsNotNull(_infoPanel);
            Assert.IsNotNull(_nameText);
            Assert.IsNotNull(_priceText);
            Assert.IsNotNull(_interactionCollider);

            _defaultMaterial = _weaponSpriteRenderer.sharedMaterial;
            _infoPanel.gameObject.SetActive(false);
        }

        public void Initialize(ShopContext context)
        {
            Assert.IsNotNull(context.ShopTable);
            Assert.IsNotNull(context.CameraController);
            Assert.IsNotNull(context.PlayerStats);
            Assert.IsNotNull(context.PlayerEquipment);
            Assert.IsNotNull(context.PlayerInventory);

            _context = context;
            RollItem();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag(GlobalConstants.PLAYER_TAG))
            {
                IsPlayerInRange = true;
                if (!Sold)
                {
                    _infoPanel.gameObject.SetActive(true);
                }
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag(GlobalConstants.PLAYER_TAG))
            {
                IsPlayerInRange = false;
                SetHighlighted(false);
                _infoPanel.gameObject.SetActive(false);
            }
        }

        public void SetHighlighted(bool highlighted)
        {
            if (highlighted != _isHighlighted)
            {
                _isHighlighted = highlighted;
                _weaponSpriteRenderer.material = highlighted ? _highlightMaterial : _defaultMaterial;
            }
        }

        public void Interact()
        {
            if (Sold)
            {
                return;
            }

            if (_currentEntry == null)
            {
                return;
            }

            if (!_context.PlayerStats.TrySpendCurrency(_currentEntry.Price))
            {
                return;
            }

            _currentEntry.Item.Apply(_context.PlayerEquipment, _context.PlayerInventory, _context.PlayerStats);
            Sold = true;

            _interactionCollider.enabled = false;
            _weaponSpriteRenderer.material = _defaultMaterial;
            _weaponSpriteRenderer.enabled = false;
            _infoPanel.gameObject.SetActive(false);
        }

        public void Restock()
        {
            Sold = false;
            IsPlayerInRange = false;
            _isHighlighted = false;
            _interactionCollider.enabled = true;
            _weaponSpriteRenderer.enabled = true;
            _weaponSpriteRenderer.material = _defaultMaterial;
            _infoPanel.gameObject.SetActive(false);
            RollItem();
        }

        private void RollItem()
        {
            var entries = _context.ShopTable.Roll(_context.PlayerEquipment, _context.PlayerInventory, 1);

            if (entries.Length > 0)
            {
                _currentEntry = entries.First();
                _weaponSpriteRenderer.sprite = _currentEntry.Item.Icon;
                _nameText.text = _currentEntry.Item.ItemName;
                _priceText.text = $"${_currentEntry.Price.ToString()}";
                PositionInfoPanel();
            }
        }

        private void PositionInfoPanel()
        {
            var localPos = _infoPanel.localPosition;
            localPos.y = _weaponSpriteRenderer.localBounds.max.y + _infoPanelYOffset;
            _infoPanel.localPosition = localPos;
        }
    }
}