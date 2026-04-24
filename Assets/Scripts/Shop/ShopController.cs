using Player;
using UnityEngine;
using UnityEngine.Assertions;
using Loadout;

namespace Shop
{
    public class ShopController : MonoBehaviour
    {
        [SerializeField] private ShopTable _shopTable;
        [SerializeField] private CameraController _cameraController;
        [SerializeField] private PlayerStatsSo _playerStats;
        [SerializeField] private PlayerEquipment _playerEquipment;
        [SerializeField] private PlayerInventory _playerInventory;

        private ShopItem[] _shopItems;
        private bool _hasFocus;

        private void Awake()
        {
            Assert.IsNotNull(_shopTable);
            Assert.IsNotNull(_cameraController);
            Assert.IsNotNull(_playerStats);
            Assert.IsNotNull(_playerEquipment);
            Assert.IsNotNull(_playerInventory);
        }

        private void Start()
        {
            _shopItems = GetComponentsInChildren<ShopItem>();

            var context = new ShopContext
            {
                ShopTable = _shopTable,
                CameraController = _cameraController,
                PlayerStats = _playerStats,
                PlayerEquipment = _playerEquipment,
                PlayerInventory = _playerInventory,
            };

            for (int i = 0; i < _shopItems.Length; i++)
            {
                Assert.IsNotNull(_shopItems[i]);
                _shopItems[i].Initialize(context);
            }
        }

        private void Update()
        {
            ShopItem closest = null;
            float closestDistanceSq = float.MaxValue;
            Vector2 playerPos = _playerEquipment.transform.position;

            for (int i = 0; i < _shopItems.Length; i++)
            {
                if (_shopItems[i].Sold || !_shopItems[i].IsPlayerInRange)
                {
                    continue;
                }

                float distanceSq = ((Vector2)_shopItems[i].transform.position - playerPos).sqrMagnitude;

                if (distanceSq < closestDistanceSq)
                {
                    closestDistanceSq = distanceSq;
                    closest = _shopItems[i];
                }
            }

            for (int i = 0; i < _shopItems.Length; i++)
            {
                if (_shopItems[i].Sold)
                {
                    continue;
                }

                _shopItems[i].SetHighlighted(_shopItems[i] == closest);
            }

            if (closest != null)
            {
                _cameraController.SetShopFocus(closest.transform.position);
                _hasFocus = true;
            }
            else if (_hasFocus)
            {
                _cameraController.ClearShopFocus();
                _hasFocus = false;
            }
        }

        public void RestockAll()
        {
            for (int i = 0; i < _shopItems.Length; i++)
            {
                _shopItems[i].Restock();
            }
        }
    }
}