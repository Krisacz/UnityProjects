using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Models
{
    public class Blueprint
    {
        public BlueprintGroup BlueprintGroup { get; private set; } 
        public string Title { get; private set; }
        public string Description { get; private set; }
        public ItemId ResultItemId { get; private set; }
        public int ResultItemCount { get; private set; }
        public float CraftingTime { get; private set; }
        public Dictionary<ItemId, int> Requirements { get; private set; } 
        
        public Blueprint(BlueprintGroup blueprintGroup, string title, string description, ItemId resultItemId,
            int resultItemCount, float craftingTime, Dictionary<ItemId, int> requirements)
        {
            BlueprintGroup = blueprintGroup;
            Title = title;
            Description = description;
            ResultItemId = resultItemId;
            ResultItemCount = resultItemCount;
            CraftingTime = craftingTime;
            Requirements = requirements;
        }
    }
}