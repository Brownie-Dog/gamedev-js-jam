using UnityEngine;

namespace ItemDrops
{
    [System.Serializable]
    public class LootEntry
    {
        [SerializeField]
        private ItemData _item;

        [SerializeField]
        private float _weight = 1.0f;

        public ItemData Item => _item;
        public float Weight => _weight;
    }
}
