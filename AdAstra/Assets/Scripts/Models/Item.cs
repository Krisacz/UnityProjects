using System;
using UnityEngine;

namespace Assets.Scripts.Models
{
    public class Item
    {
        public ItemId ItemId { get; private set; }
        public string Title { get; private set; }
        public string Description { get; private set; }
        public int MaxStackSize { get; private set; }
        public string SpriteName { get; private set; }

        //If item is a structure:
        public bool IsStructure { get; private set;}
        public StructureSize StructureSize { get; private set; }
        public bool StructureBlocking { get; private set; }
        public StructureElevation StructureElevation { get; private set; }

        public Item(ItemId itemId, string title, string description, 
            int maxStackSize, string spriteName,
            bool isStructure, StructureSize structureSize, 
            bool structureBlocking, StructureElevation structureElevation)
        {
            ItemId = itemId;
            Title = title;
            Description = description;
            MaxStackSize = maxStackSize;
            SpriteName = spriteName;

            IsStructure = isStructure;
            StructureSize = structureSize;
            StructureBlocking = structureBlocking;
            StructureElevation = structureElevation;
        }
    }
}