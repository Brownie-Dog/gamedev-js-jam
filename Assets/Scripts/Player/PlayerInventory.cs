using System.Collections.Generic;
using ItemDrops;
using UnityEngine;
using UnityEngine.Assertions;

namespace Player
{
    public class PlayerInventory : MonoBehaviour
    {
        private readonly List<ItemData> _items = new();

        public void AddItem(ItemData item)
        {
            Assert.IsNotNull(item);
            _items.Add(item);
        }

        public void RemoveItem(int index)
        {
            Assert.IsTrue(index >= 0 && index < _items.Count);
            _items.RemoveAt(index);
        }

        public bool HasWeapon(WeaponItemData weapon)
        {
            return _items.Contains(weapon);
        }

        public List<WeaponItemData> GetAllWeapons()
        {
            var weapons = new List<WeaponItemData>();
            foreach (var item in _items)
            {
                if (item is WeaponItemData weapon)
                {
                    weapons.Add(weapon);
                }
            }
            return weapons;
        }
    }
}
