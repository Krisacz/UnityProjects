using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Assets.Scripts.Models;
using LitJson;
using UnityEngine;

namespace Assets.Scripts.Db
{
    public static class BlueprintsDatabase
    {
        //TODO Consider using list instead of dic to support multiple recipies for one item
        private static readonly Dictionary<ItemId, Blueprint> BlueprintsDb = new Dictionary<ItemId, Blueprint>();
        private static readonly Dictionary<ItemFunction, List<Blueprint>> BlueprintsGroupDb = new Dictionary<ItemFunction, List<Blueprint>>();
        
        public static void Init()
        {
            var path = string.Format("{0}/StreamingAssets/blueprintsDb.jason", Application.dataPath);
            var fileContent = File.ReadAllText(path);
            var jasonData = JsonMapper.ToObject(fileContent);
            for (var i = 0; i < jasonData.Count; i++)
            {
                var blueprint = MapJsonObjectToBlueprint(jasonData[i]);

                //Blueprints DB
                BlueprintsDb.Add(blueprint.ResultItemId, blueprint);

                //Blueprints Groups DB
                if (BlueprintsGroupDb.ContainsKey(blueprint.BlueprintGroup))
                {
                    BlueprintsGroupDb[blueprint.BlueprintGroup].Add(blueprint);
                }
                else
                {
                    var list = new List<Blueprint>() {blueprint};
                    BlueprintsGroupDb.Add(blueprint.BlueprintGroup, list);
                }
            }

            //Debug log
            //foreach (var item in Database) Log.Info("BlueprintsDatabase", "Init", string.Format("Blueprint Loaded: [{0}]", item.Key));
        }

        public static Blueprint GetBlueprint(ItemId itemId)
        {
            if (BlueprintsDb.ContainsKey(itemId)) return BlueprintsDb[itemId];
            Log.Error("BlueprintsDatabase", "GetBlueprint", string.Format("Blueprint for ItemId = {0} doesn't exist in blueprints databse.", itemId));
            return null;
        }

        public static List<Blueprint> GetGroupBlueprints(ItemFunction itemFunction)
        {
            if (BlueprintsGroupDb.ContainsKey(itemFunction)) return BlueprintsGroupDb[itemFunction];
            Log.Error("BlueprintsDatabase", "GetGroupBlueprints", string.Format("BlueprintGroup = {0} doesn't exist in blueprints databse.", itemFunction));
            return new List<Blueprint>();
        }

        private static Blueprint MapJsonObjectToBlueprint(JsonData json)
        {
            var group = JHelper.AsEnum<ItemFunction>(json["BlueprintFunction"]);
            var resultItemId = JHelper.AsEnum<ItemId>(json["ResultItemId"]);
            var resultItemCount = JHelper.AsInt(json["ResultItemCount"]);
            var craftingTime = JHelper.AsFloat(json["CraftingTime"]);
            var dic = JHelper.AsDictionary<string, int>(json["Requirements"].ToJson());
            var requirements = dic.ToDictionary(d => (ItemId) Enum.Parse(typeof (ItemId), d.Key), d => d.Value);

            if(requirements.Count > 8) Log.Warn("BlueprintsDatabase", "MapJsonObjectToBlueprint",
                string.Format("ItemId = {0} requires {1} crafting components - will they fit on the screen??!?!?!",
                resultItemId, requirements.Count));

            return new Blueprint(group, resultItemId, resultItemCount, craftingTime, requirements);
        }
    }
}
