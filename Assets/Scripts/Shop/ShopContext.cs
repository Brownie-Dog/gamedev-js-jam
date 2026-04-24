using Loadout;
using Player;
using UnityEngine;

namespace Shop
{
    public struct ShopContext
    {
        public ShopTable ShopTable;
        public CameraController CameraController;
        public PlayerStatsSo PlayerStats;
        public PlayerEquipment PlayerEquipment;
        public PlayerInventory PlayerInventory;
    }
}