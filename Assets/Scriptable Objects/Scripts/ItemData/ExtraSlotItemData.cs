using ItemDrops;
using Player;
using UnityEngine;

namespace ItemDrops
{
    [CreateAssetMenu(
        fileName = nameof(ExtraSlotItemData),
        menuName = "ScriptableObjects/Items/Extra Slot"
    )]
    public class ExtraSlotItemData : ItemData
    {
        public override bool CanDrop(PlayerEquipment equipment, PlayerInventory inventory)
        {
            return equipment.UnlockedSlots < equipment.MaxSlots;
        }

        public override void Apply(PlayerEquipment equipment, PlayerInventory inventory, PlayerStatsSo stats)
        {
            equipment.UnlockSlot();
        }
    }
}