using ItemDrops;
using Player;
using UnityEngine;
using Weapons;

namespace ItemDrops
{
    [CreateAssetMenu(
        fileName = nameof(StatBoostItemData),
        menuName = "ScriptableObjects/Items/Stat Boost"
    )]
    public class StatBoostItemData : ItemData
    {
        [field: SerializeField]
        public string StatName { get; private set; } = string.Empty;

        [field: SerializeField]
        public float Value { get; private set; } = 0f;

        public override void Apply(PlayerEquipment equipment, PlayerInventory inventory)
        {
            Debug.Log($"Applied {StatName} boost: {Value}");
        }
    }
}
