using System;
using Player;
using UnityEngine;
using UnityEngine.Assertions;
using Weapons;

namespace ItemDrops
{
    public class ItemDropManager : MonoBehaviour
    {
        [SerializeField]
        private ItemChoiceScreen _chooseItemScreen;

        [SerializeField]
        private GameObject _itemDropPrefab;

        [SerializeField]
        private LootTable _lootTable;

        [SerializeField]
        private PlayerEquipment _playerEquipment;

        [SerializeField]
        private PlayerInventory _playerInventory;

        [SerializeField]
        private PlayerStatsSo _playerStats;

        private bool _guaranteeLegendary;

        private void Awake()
        {
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

        public void SpawnItemDropObject(Vector2 worldPosition, bool guaranteeLegendary = false)
        {
            _guaranteeLegendary = guaranteeLegendary;

            var droppedItem = Instantiate(
                _itemDropPrefab,
                worldPosition,
                Quaternion.identity,
                gameObject.transform
            );

            var droppedItemComponent = droppedItem.GetComponent<DroppedItem>();
            Assert.IsNotNull(droppedItemComponent);
            droppedItemComponent.GuaranteeLegendary = guaranteeLegendary;
        }

        public void OnItemPickup(GameObject droppedItem, bool guaranteeLegendary)
        {
            _guaranteeLegendary = guaranteeLegendary;

            var items = _lootTable.Roll(_guaranteeLegendary, _playerEquipment, _playerInventory);
            _chooseItemScreen.Show(items);

            Destroy(droppedItem);
        }

        private void HandleItemPicked(object sender, ItemPickedEventArgs e)
        {
            e.Item.Apply(_playerEquipment, _playerInventory, _playerStats);
        }

        private void HandleRerollRequested(object sender, EventArgs e)
        {
            var items = _lootTable.Roll(_guaranteeLegendary, _playerEquipment, _playerInventory);
            _chooseItemScreen.Reroll(items);
        }
    }
}
