using UnityEngine;

namespace ItemDrops
{
    [System.Serializable]
    public class LootEntry
    {
        [field: SerializeField]
        public ItemData Item { get; private set; } = null;

        [field: SerializeField]
        public float Weight { get; private set; } = 1.0f;
    }
}
