using System;
using ItemDrops;
using Player;
using UnityEngine;
using UnityEngine.Assertions;

namespace ItemDrops
{
    [CreateAssetMenu(
        fileName = nameof(StatBoostItemData),
        menuName = "ScriptableObjects/Items/Stat Boost"
    )]
    public class StatBoostItemData : ItemData
    {
        [field: SerializeField]
        public StatType Stat { get; private set; } = StatType.None;

        [field: SerializeField]
        public float Value { get; private set; } = 0f;

        public override void Apply(
            PlayerEquipment equipment,
            PlayerInventory inventory,
            PlayerStatsSo stats
        )
        {
            Assert.IsNotNull(stats);

            switch (Stat)
            {
                case StatType.MovementSpeed:
                    stats.MovementSpeed += Value;
                    break;
                case StatType.MaxHealth:
                    stats.MaxHealth += (int)Value;
                    stats.UpdateHealth(stats.MaxHealth);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
