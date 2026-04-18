using ItemDrops;
using Player;
using UnityEngine;
using Weapons;

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

        public override void Apply(PlayerEquipment equipment, PlayerInventory inventory)
        {
            var emptySlot = equipment.FirstEmptySlot();

            if (emptySlot >= 0)
            {
                equipment.Equip(this, emptySlot);
                return;
            }

            inventory.AddItem(this);
        }
    }
}
