using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Models
{
    public class Blueprint
    {
        public ItemId ResultItemId { get; private set; }
        public int ResultItemCount { get; private set; }
        public float CraftingTime { get; private set; }
        public Dictionary<ItemId, int> Requirements { get; private set; } 
        
        public Blueprint(ItemId resultItemId, int resultItemCount, float craftingTime, Dictionary<ItemId, int> requirements)
        {
            ResultItemId = resultItemId;
            ResultItemCount = resultItemCount;
            CraftingTime = craftingTime;
            Requirements = requirements;
        }
    }
}