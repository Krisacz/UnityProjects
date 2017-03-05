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
        private static readonly List<Blueprint> BlueprintsDb = new List<Blueprint>();

        public static void Init()
        {
            var path = string.Format("{0}/StreamingAssets/blueprintsDb.json", Application.dataPath);
            var fileContent = File.ReadAllText(path);
            var jasonData = JsonMapper.ToObject(fileContent);
            for (var i = 0; i < jasonData.Count; i++)
            {
                var blueprint = MapJsonObjectToBlueprint(jasonData[i]);

                //Blueprints DB
                BlueprintsDb.Add(blueprint);
            }

            //Debug log
            //foreach (var item in Database) Log.Info("BlueprintsDatabase", "Init", string.Format("Blueprint Loaded: [{0}]", item.Key));
        }

        public static Blueprint GetBlueprint(ItemId itemId)
        {
            var blueprint = BlueprintsDb.FirstOrDefault(x => x.ResultItemId == itemId);
            if (blueprint != null && blueprint.ResultItemId != ItemId.None) return blueprint;
            Log.Error("BlueprintsDatabase", "GetBlueprint", string.Format("Blueprint for ItemId = {0} doesn't exist in blueprints databse.", itemId));
            return null;
        }

        private static Blueprint MapJsonObjectToBlueprint(JsonData json)
        {
            try
            {
                var resultItemId = JHelper.AsEnum<ItemId>(json["ResultItemId"]);
                var resultItemCount = JHelper.AsInt(json["ResultItemCount"]);
                var craftingTime = JHelper.AsFloat(json["CraftingTime"]);
                var dicStr = JHelper.AsDictionary<string, string>(json["Requirements"].ToJson());
                var requirements = dicStr.ToDictionary(d => (ItemId)Enum.Parse(typeof(ItemId), d.Key), d => int.Parse(d.Value));

                if (requirements.Count > 8) Log.Warn("BlueprintsDatabase", "MapJsonObjectToBlueprint",
                     string.Format("ItemId = {0} requires {1} crafting components - will they fit on the screen??!?!?!",
                     resultItemId, requirements.Count));

                return new Blueprint(resultItemId, resultItemCount, craftingTime, requirements);
            }
            catch (Exception ex)
            {
                var a = 1;
            }

            return null;
        }
    }
}
