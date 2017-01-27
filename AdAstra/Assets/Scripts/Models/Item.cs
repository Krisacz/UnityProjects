using System;
using System.Collections.Generic;
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
        public Function Function { get; private set; }
        public Dictionary<FunctionProperty, string> FunctionProperties { get; private set; }
        
        public Item(ItemId itemId, string title, string description, int maxStackSize, string sprite,
            Function function, Dictionary<FunctionProperty, string> functionProperties)
        {
            ItemId = itemId;
            Title = title;
            Description = description;
            MaxStackSize = maxStackSize;
            SpriteName = sprite;

            Function = function;
            FunctionProperties = functionProperties;
        }
    }
}