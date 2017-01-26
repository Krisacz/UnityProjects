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
            var path = string.Format("{0}/StreamingAssets/itemsDb.jason", Application.dataPath);
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

        private static Item MapJsonObjectToItem(JsonData json)
        {
            var itemId = JsonHelper.AsEnum<ItemId>(json["ItemId"]);
            var title = JsonHelper.AsString(json["Title"]);
            var description = JsonHelper.AsString(json["Description"]);
            var maxStackSize = JsonHelper.AsInt(json["MaxStackSize"]);
            var spriteName = JsonHelper.AsString(json["SpriteName"]);

            var isStructure = JsonHelper.AsBool(json["IsStructure"]);
            var structureSize = new StructureSize(JsonHelper.AsInt(json["StructureSize"]["Width"]),
                JsonHelper.AsInt(json["StructureSize"]["Height"]));
            var structureBlocking = JsonHelper.AsBool(json["StructureBlocking"]);
            var structureElevation = JsonHelper.AsEnum<StructureElevation>(json["StructureElevation"]);
            var constructionTime = JsonHelper.AsFloat(json["ConstructionTime"]);

            return new Item(itemId, title, description, maxStackSize, spriteName, isStructure,
                structureSize, structureBlocking, structureElevation, constructionTime);
        }
    }
}
