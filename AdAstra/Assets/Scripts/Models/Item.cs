using System;
using UnityEngine;

namespace Assets.Scripts.Models
{
    public class Item
    {
        public ItemId ItemId { get; private set; }
        public ItemSize ItemSize { get; private set; }
        public int MaxStackSize { get; private set; }
        public string SpriteName { get; private set; }
        public string PrefabName { get; private set; }
        
        public Item(ItemId itemId, ItemSize itemSize, int maxStackSize, string spriteName, string prefabName)
        {
            ItemId = itemId;
            MaxStackSize = maxStackSize;
            SpriteName = spriteName;
            PrefabName = prefabName;
            ItemSize = itemSize;
        }
    }
}