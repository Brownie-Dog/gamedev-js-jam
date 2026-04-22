using System.Collections.Generic;
using System.Linq;
using Player;
using UnityEngine;
using UnityEngine.Assertions;

namespace ItemDrops
{
    [CreateAssetMenu(fileName = nameof(LootTable), menuName = "ScriptableObjects/Loot Table")]
    public class LootTable : ScriptableObject
    {
        [SerializeField]
        private List<LootEntry> _items = new();

        [SerializeField]
        private float _rareWeight = 70.0f;

        [SerializeField]
        private float _epicWeight = 25.0f;

        [SerializeField]
        private float _legendaryWeight = 5.0f;

        public int RollCount => 3;

        public ItemData[] Roll(PlayerEquipment equipment, PlayerInventory inventory)
        {
            Assert.IsNotNull(equipment);
            Assert.IsNotNull(inventory);

            var results = new List<ItemData>();

            for (int i = 0; i < RollCount; i++)
            {
                var rarity = RollRarity();

                var filteredPool = FilterPool(_items, equipment, inventory, rarity);

                var fallbackRarity = GetFallbackRarity(rarity);
                while (filteredPool.Count == 0 && fallbackRarity != Rarity.Rare)
                {
                    filteredPool = FilterPool(_items, equipment, inventory, fallbackRarity);
                    fallbackRarity = GetFallbackRarity(fallbackRarity);
                }

                if (filteredPool.Count > 0)
                {
                    var item = RollFromPool(filteredPool);
                    results.Add(item);
                }
            }

            return results.ToArray();
        }

        private Rarity RollRarity()
        {
            var total = _rareWeight + _epicWeight + _legendaryWeight;
            var roll = Random.Range(0f, total);

            if (roll < _legendaryWeight)
            {
                return Rarity.Legendary;
            }

            if (roll < _legendaryWeight + _epicWeight)
            {
                return Rarity.Epic;
            }

            return Rarity.Rare;
        }

        private List<LootEntry> FilterPool(
            List<LootEntry> pool,
            PlayerEquipment equipment,
            PlayerInventory inventory,
            Rarity rarity
        )
        {
            var filtered = new List<LootEntry>();

            foreach (var entry in pool)
            {
                if (entry.Item == null)
                {
                    continue;
                }

                if (entry.Item.Rarity != rarity)
                {
                    continue;
                }

                if (!entry.Item.CanDrop(equipment, inventory))
                {
                    continue;
                }

                filtered.Add(entry);
            }

            return filtered;
        }

        private ItemData RollFromPool(List<LootEntry> pool)
        {
            var totalWeight = pool.Sum(e => e.Weight);
            var roll = Random.Range(0f, totalWeight);

            foreach (var entry in pool)
            {
                if (roll < entry.Weight)
                {
                    return entry.Item;
                }

                roll -= entry.Weight;
            }

            return pool[0].Item;
        }

        private Rarity GetFallbackRarity(Rarity rarity)
        {
            return rarity switch
            {
                Rarity.Legendary => Rarity.Epic,
                Rarity.Epic => Rarity.Rare,
                _ => Rarity.Rare,
            };
        }
    }
}