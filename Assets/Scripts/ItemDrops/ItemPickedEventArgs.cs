using System;
using UnityEngine;

namespace ItemDrops
{
    public class ItemPickedEventArgs : EventArgs
    {
        public ItemData Item { get; }
        public Sprite IconSprite { get; }
        public Vector2 IconScreenPosition { get; }

        public ItemPickedEventArgs(ItemData item, Sprite iconSprite, Vector2 iconScreenPosition)
        {
            Item = item;
            IconSprite = iconSprite;
            IconScreenPosition = iconScreenPosition;
        }
    }
}
