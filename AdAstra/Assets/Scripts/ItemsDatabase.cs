using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using LitJson;
using Assets.Scripts.Models;
using UnityEngine;

namespace Assets.Scripts
{
    public static class ItemsDatabase
    {
        private static readonly Dictionary<ItemId, Item> Database = new Dictionary<ItemId, Item>();
        
        public static void Init()
        {
            var path = string.Format("{0}/StreamingAssets/itemsDb.jason", Application.dataPath);
            var fileContent = File.ReadAllText(path);
            var jasonData = JsonMapper.ToObject(fileContent);
            for (var i = 0; i < jasonData.Count; i++)
            {
                var item = MapJsonObjectToItem(jasonData[i]);
                Database.Add(item.ItemId, item);
            }

            //Debug log
            foreach (var item in Database) Log.Info("ItemsDatabase", "Init", string.Format("Item Loaded: [{0}]", item.Key));
        }

        public static Item Get(ItemId itemId)
        {
            if (Database.ContainsKey(itemId)) return Database[itemId];
            Log.Error("ItemsDatabase", "Get", string.Format("ItemId = {0} doesn't exist in items databse.", itemId));
            return null;
        }

        private static Item MapJsonObjectToItem(JsonData json)
        {
            var itemId = AsEnum<ItemId>(json["ItemId"]);
            var maxStackSize = AsInt(json["MaxStackSize"]);
            var itemSize = new ItemSize(AsInt(json["ItemSize"]["Width"]), AsInt(json["ItemSize"]["Height"]));
            var spriteName = AsString(json["SpriteName"]);
            var prefabName = AsString(json["PrefabName"]);
            return new Item(itemId, itemSize, maxStackSize, spriteName, prefabName);
        }

        private static string AsString(IEnumerable data)
        {
            return data.ToString();
        }

        private static int AsInt(IEnumerable data)
        {
            return int.Parse(data.ToString());
        }

        private static T AsEnum<T>(IEnumerable data)
        {
            return (T) Enum.Parse(typeof (T), AsString(data));
        }
    }
}
