using System;

namespace ItemDrops
{
    public class ItemPickedEventArgs : EventArgs
    {
        public ItemData Item { get; }

        public ItemPickedEventArgs(ItemData item)
        {
            Item = item;
        }
    }
}