using ItemDrops;
using Player;
using UnityEngine;

namespace ItemDrops
{
    [CreateAssetMenu(
        fileName = nameof(WeaponItemData),
        menuName = "ScriptableObjects/Items/Weapon"
    )]
    public class WeaponItemData : ItemData
    {
        [field: SerializeField]
        public GameObject WeaponPrefab { get; private set; } = null;

        [field: SerializeField]
        public int Damage { get; private set; } = 1;

        [field: SerializeField]
        public float CooldownTime { get; private set; } = 0f;

        public override void Apply(PlayerEquipment equipment, PlayerInventory inventory, PlayerStatsSo stats)
        {
            var emptySlot = equipment.FirstEmptySlot();

            if (emptySlot >= 0)
            {
                equipment.Equip(this, emptySlot);
                return;
            }

            Debug.Log($"No empty equipment slot for {ItemName}, adding to inventory");
            inventory.AddItem(this);
        }

        public void ApplyUpgrade(float damageMultiplier, float speedMultiplier)
        {
            Damage = (int)(Damage * damageMultiplier);
            CooldownTime *= speedMultiplier;
        }
    }
}
