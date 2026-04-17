using NUnit.Framework;
using UnityEngine;

namespace ItemDrops
{
    public class ItemDropManager : MonoBehaviour
    {
        [SerializeField]
        private ItemChoiceScreen _chooseItemScreen;

        [SerializeField]
        private GameObject _itemDropPrefab;

        private void Awake()
        {
            Assert.IsNotNull(_chooseItemScreen);
            Assert.IsNotNull(_itemDropPrefab);
        }

        public void SpawnItemDropObject(Vector2 worldPosition)
        {
            var droppedItem = Instantiate(
                _itemDropPrefab,
                worldPosition,
                Quaternion.identity,
                gameObject.transform
            );
        }

        public void OnItemPickup(GameObject droppedItem)
        {
            _chooseItemScreen.Show();

            Destroy(droppedItem);
        }
    }
}
