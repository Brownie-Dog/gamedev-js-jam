using System;
using Player;
using UnityEngine;
using UnityEngine.Assertions;
using Random = UnityEngine.Random;

namespace ItemDrops
{
    public class ItemDropManager : MonoBehaviour
    {
        public static ItemDropManager Instance { get; private set; }

        [SerializeField] private ItemChoiceScreen _chooseItemScreen;

        [SerializeField] private GameObject _itemDropPrefab;

        [SerializeField] private LootTable _lootTable;

        [SerializeField] private PlayerEquipment _playerEquipment;

        [SerializeField] private PlayerInventory _playerInventory;

        [SerializeField] private PlayerStatsSo _playerStats;

        private void Awake()
        {
            Assert.IsNull(Instance);
            Instance = this;

            Assert.IsNotNull(_chooseItemScreen);
            Assert.IsNotNull(_itemDropPrefab);
            Assert.IsNotNull(_lootTable);
            Assert.IsNotNull(_playerEquipment);
            Assert.IsNotNull(_playerInventory);
            Assert.IsNotNull(_playerStats);
        }

        private void OnEnable()
        {
            _chooseItemScreen.ItemPicked += HandleItemPicked;
            _chooseItemScreen.RerollRequested += HandleRerollRequested;
        }

        private void OnDisable()
        {
            _chooseItemScreen.ItemPicked -= HandleItemPicked;
            _chooseItemScreen.RerollRequested -= HandleRerollRequested;
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }

        public void SpawnItemDropObject(Vector2 worldPosition, ItemData guaranteedItem = null)
        {
            var droppedItem = Instantiate(_itemDropPrefab, worldPosition, Quaternion.identity, gameObject.transform);

            var droppedItemComponent = droppedItem.GetComponent<DroppedItem>();
            Assert.IsNotNull(droppedItemComponent);
            droppedItemComponent.GuaranteedItem = guaranteedItem;
        }

        public void OnItemPickup(GameObject droppedItem, ItemData guaranteedItem)
        {
            var items = _lootTable.Roll(_playerEquipment, _playerInventory);

            if (guaranteedItem != null && items.Length > 0)
            {
                var slot = Random.Range(0, items.Length);
                items[slot] = guaranteedItem;
            }

            _chooseItemScreen.Show(items);

            Destroy(droppedItem);
        }

        private void HandleItemPicked(object sender, ItemPickedEventArgs e)
        {
            e.Item.Apply(_playerEquipment, _playerInventory, _playerStats);
        }

        private void HandleRerollRequested(object sender, EventArgs e)
        {
            var items = _lootTable.Roll(_playerEquipment, _playerInventory);
            _chooseItemScreen.Reroll(items);
        }
    }
}