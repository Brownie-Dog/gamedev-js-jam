using ItemDrops;
using UnityEngine;

namespace Shop
{
    [System.Serializable]
    public class ShopEntry
    {
        [SerializeField] private ItemData _item;
        [SerializeField] private int _price;
        [SerializeField] private float _weight = 1f;

        public ItemData Item => _item;
        public int Price => _price;
        public float Weight => _weight;
    }
}