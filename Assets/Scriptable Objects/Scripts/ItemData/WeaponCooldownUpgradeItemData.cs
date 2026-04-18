using ItemDrops;
using Player;
using UnityEngine;
using UnityEngine.Assertions;

namespace ItemDrops
{
    [CreateAssetMenu(
        fileName = nameof(WeaponCooldownUpgradeItemData),
        menuName = "ScriptableObjects/Items/Weapon Cooldown Upgrade"
    )]
    public class WeaponCooldownUpgradeItemData : ItemData
    {
        [field: SerializeField]
        public WeaponItemData TargetWeapon { get; private set; } = null;

        [field: SerializeField]
        public float CooldownMultiplier { get; private set; } = 1.0f;

        protected void OnValidate()
        {
            Assert.IsNotNull(TargetWeapon, $"{name}: TargetWeapon must be set.");
            Assert.IsTrue(CooldownMultiplier != 1.0f, $"{name}: CooldownMultiplier must differ from 1.0.");
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
                    weapon.MultiplyCooldown(CooldownMultiplier);
                }
            }

            var inventoryWeapons = inventory.GetAllWeapons();
            foreach (var weapon in inventoryWeapons)
            {
                if (weapon == TargetWeapon)
                {
                    weapon.MultiplyCooldown(CooldownMultiplier);
                }
            }
        }
    }
}