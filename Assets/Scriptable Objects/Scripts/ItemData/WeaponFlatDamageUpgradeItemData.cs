using ItemDrops;
using Player;
using UnityEngine;
using UnityEngine.Assertions;

namespace ItemDrops
{
    [CreateAssetMenu(
        fileName = nameof(WeaponFlatDamageUpgradeItemData),
        menuName = "ScriptableObjects/Items/Weapon Flat Damage Upgrade"
    )]
    public class WeaponFlatDamageUpgradeItemData : ItemData
    {
        [field: SerializeField]
        public WeaponItemData TargetWeapon { get; private set; } = null;

        [field: SerializeField]
        public int FlatDamage { get; private set; } = 0;

        protected void OnValidate()
        {
            Assert.IsNotNull(TargetWeapon, $"{name}: TargetWeapon must be set.");
            Assert.IsTrue(FlatDamage > 0, $"{name}: FlatDamage must be greater than 0.");
        }

        public override bool CanDrop(PlayerEquipment equipment, PlayerInventory inventory)
        {
            var ownedWeapons = equipment.GetAllOwnedWeapons();
            var inventoryWeapons = inventory.GetAllWeapons();
            return ownedWeapons.Contains(TargetWeapon) || inventoryWeapons.Contains(TargetWeapon);
        }

        public override void Apply(PlayerEquipment equipment, PlayerInventory inventory, PlayerStatsSo stats)
        {
            Assert.IsNotNull(TargetWeapon);

            var equippedWeapons = equipment.GetAllOwnedWeapons();
            foreach (var weapon in equippedWeapons)
            {
                if (weapon == TargetWeapon)
                {
                    weapon.AddFlatDamage(FlatDamage);
                }
            }

            var inventoryWeapons = inventory.GetAllWeapons();
            foreach (var weapon in inventoryWeapons)
            {
                if (weapon == TargetWeapon)
                {
                    weapon.AddFlatDamage(FlatDamage);
                }
            }
        }
    }
}