using System.Collections.Generic;
using System.Linq;
using ItemDrops;
using Player;
using UnityEngine;

namespace Shop
{
    [CreateAssetMenu(fileName = nameof(ShopTable), menuName = "ScriptableObjects/Shop Table")]
    public class ShopTable : ScriptableObject
    {
        [SerializeField] private List<ShopEntry> _items = new();
        [SerializeField] private float _rareWeight = 70f;
        [SerializeField] private float _epicWeight = 25f;
        [SerializeField] private float _legendaryWeight = 5f;

        public ShopEntry[] Roll(PlayerEquipment equipment, PlayerInventory inventory, int count)
        {
            var results = new List<ShopEntry>();
            var used = new HashSet<ItemData>();

            for (int i = 0; i < count; i++)
            {
                var rarity = RollRarity();
                var filteredPool = FilterPool(_items, equipment, inventory, rarity, used);

                var fallbackRarity = GetFallbackRarity(rarity);
                while (filteredPool.Count == 0 && fallbackRarity != Rarity.Rare)
                {
                    filteredPool = FilterPool(_items, equipment, inventory, fallbackRarity, used);
                    fallbackRarity = GetFallbackRarity(fallbackRarity);
                }

                if (filteredPool.Count > 0)
                {
                    var entry = RollFromPool(filteredPool);
                    results.Add(entry);
                    used.Add(entry.Item);
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

        private static List<ShopEntry> FilterPool(List<ShopEntry> pool, PlayerEquipment equipment, PlayerInventory inventory,
            Rarity rarity, HashSet<ItemData> used)
        {
            var filtered = new List<ShopEntry>();

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

                if (used.Contains(entry.Item))
                {
                    continue;
                }

                filtered.Add(entry);
            }

            return filtered;
        }

        private static ShopEntry RollFromPool(List<ShopEntry> pool)
        {
            var totalWeight = pool.Sum(e => e.Weight);
            var roll = Random.Range(0f, totalWeight);

            foreach (var entry in pool)
            {
                if (roll < entry.Weight)
                {
                    return entry;
                }

                roll -= entry.Weight;
            }

            return pool[0];
        }

        private static Rarity GetFallbackRarity(Rarity rarity)
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