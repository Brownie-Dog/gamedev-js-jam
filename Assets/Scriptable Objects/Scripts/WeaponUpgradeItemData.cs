using ItemDrops;
using Player;
using UnityEngine;
using UnityEngine.Assertions;

namespace ItemDrops
{
    [CreateAssetMenu(
        fileName = nameof(WeaponUpgradeItemData),
        menuName = "ScriptableObjects/Items/Weapon Upgrade"
    )]
    public class WeaponUpgradeItemData : ItemData
    {
        [field: SerializeField]
        public WeaponItemData TargetWeapon { get; private set; } = null;

        [field: SerializeField]
        public float DamageMultiplier { get; private set; } = 1.0f;

        [field: SerializeField]
        public float SpeedMultiplier { get; private set; } = 1.0f;

        protected void OnValidate()
        {
            Assert.IsNotNull(TargetWeapon, $"{name}: TargetWeapon must be set.");
        }

        public override void Apply(PlayerEquipment equipment, PlayerInventory inventory, PlayerStatsSo stats)
        {
            if (TargetWeapon == null)
            {
                return;
            }

            var equippedWeapons = equipment.GetAllOwnedWeapons();
            foreach (var weapon in equippedWeapons)
            {
                if (weapon == TargetWeapon)
                {
                    weapon.ApplyUpgrade(DamageMultiplier, SpeedMultiplier);
                }
            }

            var inventoryWeapons = inventory.GetAllWeapons();
            foreach (var weapon in inventoryWeapons)
            {
                if (weapon == TargetWeapon)
                {
                    weapon.ApplyUpgrade(DamageMultiplier, SpeedMultiplier);
                }
            }
        }
    }
}
