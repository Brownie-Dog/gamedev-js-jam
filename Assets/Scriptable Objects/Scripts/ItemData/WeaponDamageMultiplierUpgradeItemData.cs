using ItemDrops;
using Player;
using UnityEngine;
using UnityEngine.Assertions;

namespace ItemDrops
{
    [CreateAssetMenu(
        fileName = nameof(WeaponDamageMultiplierUpgradeItemData),
        menuName = "ScriptableObjects/Items/Weapon Damage Multiplier Upgrade"
    )]
    public class WeaponDamageMultiplierUpgradeItemData : ItemData
    {
        [field: SerializeField]
        public WeaponItemData TargetWeapon { get; private set; } = null;

        [field: SerializeField]
        public float DamageMultiplier { get; private set; } = 1.0f;

        protected void OnValidate()
        {
            Assert.IsNotNull(TargetWeapon, $"{name}: TargetWeapon must be set.");
            Assert.IsTrue(DamageMultiplier != 1.0f, $"{name}: DamageMultiplier must differ from 1.0.");
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
                    weapon.MultiplyDamage(DamageMultiplier);
                }
            }

            var inventoryWeapons = inventory.GetAllWeapons();
            foreach (var weapon in inventoryWeapons)
            {
                if (weapon == TargetWeapon)
                {
                    weapon.MultiplyDamage(DamageMultiplier);
                }
            }
        }
    }
}