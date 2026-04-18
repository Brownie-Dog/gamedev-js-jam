using ItemDrops;
using Player;
using UnityEngine;
using UnityEngine.Assertions;
using Weapons;

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

        public override void Apply(PlayerEquipment equipment, PlayerInventory inventory)
        {
            var equippedWeapons = equipment.GetAllOwnedWeapons();
            foreach (var weapon in equippedWeapons)
            {
                if (weapon == TargetWeapon)
                {
                    ApplyUpgradeToWeapon(weapon);
                }
            }

            var inventoryWeapons = inventory.GetAllWeapons();
            foreach (var weapon in inventoryWeapons)
            {
                if (weapon == TargetWeapon)
                {
                    ApplyUpgradeToWeapon(weapon);
                }
            }
        }

        private void ApplyUpgradeToWeapon(WeaponItemData weapon)
        {
            Debug.Log($"Applied upgrade to weapon: {weapon.ItemName}");
        }
    }
}
