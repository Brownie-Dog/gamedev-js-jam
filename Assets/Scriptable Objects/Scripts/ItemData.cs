using Player;
using UnityEngine;

namespace ItemDrops
{
    public abstract class ItemData : ScriptableObject
    {
        [field: SerializeField]
        public string ItemName { get; private set; } = string.Empty;

        [field: SerializeField]
        public string Description { get; private set; } = string.Empty;

        [field: SerializeField]
        public Sprite Icon { get; private set; } = null;

        [field: SerializeField]
        public Rarity Rarity { get; private set; } = Rarity.Rare;

        public abstract bool CanDrop(PlayerEquipment equipment, PlayerInventory inventory);

        public abstract void Apply(PlayerEquipment equipment, PlayerInventory inventory, PlayerStatsSo stats);
    }
}
