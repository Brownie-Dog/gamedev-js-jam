using Player;
using UnityEngine;

namespace ItemDrops
{
    [CreateAssetMenu(
        fileName = nameof(ExtensionWeaponItemData),
        menuName = "ScriptableObjects/Items/Extension Weapon"
    )]
    public class ExtensionWeaponItemData : WeaponItemData
    {
        public override bool CanDrop(PlayerEquipment equipment, PlayerInventory inventory)
        {
            // return equipment.FirstEmptySlot(this) >= 0;
            return true;
        }

        public override void Apply(
            PlayerEquipment equipment,
            PlayerInventory inventory,
            PlayerStatsSo stats
        )
        {
            var emptySlot = equipment.FirstEmptySlot(this);

            if (emptySlot >= 0)
            {
                equipment.Equip(this, emptySlot);
                return;
            }

            Debug.Log($"No empty equipment slot for {ItemName}, adding to inventory");
            inventory.AddItem(this);
        }
    }
}