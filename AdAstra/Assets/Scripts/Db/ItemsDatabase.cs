using System.Collections.Generic;
using System.IO;
using Assets.Scripts.Models;
using LitJson;
using UnityEngine;

namespace Assets.Scripts.Db
{
    public static class ItemsDatabase
    {
        private static readonly Dictionary<ItemId, Item> Database = new Dictionary<ItemId, Item>();
        
        public static void Init()
        {
            var path = string.Format("{0}/StreamingAssets/itemsDb.json", Application.dataPath);
            var fileContent = File.ReadAllText(path);
            var jasonData = JsonMapper.ToObject(fileContent);
            for (var i = 0; i < jasonData.Count; i++)
            {
                var item = MapJsonObjectToItem(jasonData[i]);
                Database.Add(item.ItemId, item);
            }

            //Debug log
            //foreach (var item in Database) Log.Info("ItemsDatabase", "Init", string.Format("Item Loaded: [{0}]", item.Key));
        }

        public static Item Get(ItemId itemId)
        {
            if (Database.ContainsKey(itemId)) return Database[itemId];
            Log.Error("ItemsDatabase", "Get", string.Format("ItemId = {0} doesn't exist in items databse.", itemId));
            return null;
        }

        public static List<ItemId> GetAllWithFunctionProperty(FunctionProperty functionProperty)
        {
            var list = new List<ItemId>();
            foreach (var item in Database)
            {
                if (!item.Value.FunctionProperties.ContainsKey(functionProperty)) continue;
                list.Add(item.Key);
            }
            return list;
        }

        private static Item MapJsonObjectToItem(JsonData json)
        {
            var itemId = JHelper.AsEnum<ItemId>(json["ItemId"]);
            var title = JHelper.AsString(json["Title"]);
            var description = JHelper.AsString(json["Description"]);
            var maxStackSize = JHelper.AsInt(json["MaxStackSize"]);
            var spriteName = JHelper.AsString(json["SpriteName"]);
            var function = JHelper.AsEnum<ItemFunction>(json["ItemFunction"]);
            var properties = JHelper.AsFuncProperty(json["FunctionProperties"].ToJson());
            
            return new Item(itemId, title, description, maxStackSize, spriteName, 
                function, properties);
        }
    }
}
