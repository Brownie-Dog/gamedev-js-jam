using System.Linq;
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
    public class WeaponItemData : ItemData, IWeaponData
    {
        [field: SerializeField]
        public GameObject WeaponPrefab { get; private set; } = null;

        [field: SerializeField]
        public int Damage { get; private set; } = 1;

        [field: SerializeField]
        public float CooldownTime { get; private set; } = 0f;

        [field: SerializeField]
        public AimMode DefaultAimMode { get; private set; } = AimMode.None;

        [field: SerializeField]
        public bool AutoFire { get; private set; } = false;

        [field: SerializeField]
        public float KnockbackForce { get; private set; } = 0f;

        [field: SerializeField]
        public SlotType[] CompatibleSlotTypes { get; private set; } = { SlotType.General };

        public override bool CanDrop(PlayerEquipment equipment, PlayerInventory inventory) => true;

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

        public void AddFlatDamage(int damageIncrease)
        {
            Damage += damageIncrease;
        }

        public void MultiplyDamage(float multiplier)
        {
            Damage = (int)(Damage * multiplier);
        }

        public void MultiplyCooldown(float multiplier)
        {
            CooldownTime *= multiplier;
        }
    }
}