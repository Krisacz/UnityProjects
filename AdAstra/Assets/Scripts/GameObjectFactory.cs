using System.Net.Mime;
using Assets.Scripts.Controllers;
using Assets.Scripts.Db;
using Assets.Scripts.Models;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public static class GameObjectFactory
    {
        public static void FromBlueprintGroup(BlueprintGroup group, Transform parent)
        {
            var go = FromPrefab("BlueprintGroup");

            go.GetComponent<Image>().sprite = SpritesDatabase.Get(group.ToString());
            go.transform.FindChild("GroupNameText").GetComponent<Text>().text = group.ToString();
            go.GetComponent<BlueprintGroupController>().SetBlueprintGroup(group);
            go.transform.SetParent(parent);

            //Set name for debuging
            var gameObjectName = string.Format("BlueprintGroup [{0}]", group);
            go.name = gameObjectName;
        }

        public static GameObject FromItemBlueprint(Blueprint blueprint, Transform parent)
        {
            var go = FromPrefab("ItemBlueprint");

            var spriteName = ItemsDatabase.Get(blueprint.ResultItemId).SpriteName;
            go.GetComponent<Image>().sprite = SpritesDatabase.Get(spriteName);
            go.GetComponent<ItemBlueprintController>().SetBlueprint(blueprint);
            go.transform.SetParent(parent);

            //Set name for debuging
            var gameObjectName = string.Format("ItemBlueprint [{0}]", blueprint.ResultItemId);
            go.name = gameObjectName;

            return go;
        }

        public static GameObject FromBlueprintRequiredItem(ItemId itemId, int count, Transform parent)
        {
            var go = FromPrefab("RequiredItem");

            var spriteName = ItemsDatabase.Get(itemId).SpriteName;
            go.GetComponent<Image>().sprite = SpritesDatabase.Get(spriteName);
            go.transform.SetParent(parent);

            //Set name for debuging
            var gameObjectName = string.Format("RequiredItem [{0}]x{1}", itemId, count);
            go.name = gameObjectName;

            return go;
        }

        public static GameObject FromItemId(ItemId itemId)
        {
            var item = ItemsDatabase.Get(itemId);
            return FromItem(item);
        }

        public static GameObject FromPrefab(string prefabName)
        {
            //Get prefab
            var prefabPath = string.Format("{0}/{1}", "Prefabs", prefabName);
            var prefab = Resources.Load(prefabPath);
            if (prefab == null)
            {
                Log.Warn("GameObjectFactory", "FromPrefab", string.Format("Item prefab = {0} doesn't exist.", prefabName));
                return null;
            }

            //Create game object
            var go = Object.Instantiate(prefab, Vector3.zero, Quaternion.identity) as GameObject;
            if (go == null)
            {
                Log.Warn("GameObjectFactory", "FromPrefab", string.Format("Game Object from prefab = {0} is null.", prefabName));
                return null;
            }

            //Return game object
            return go;
        }

        private static GameObject FromItem(Item item)
        {
            var go = FromPrefab(item.PrefabName);

            //Set name for debuging
            var gameObjectName = string.Format("Item ID[{0}]", item.ItemId);
            go.name = gameObjectName;

            //Return game object
            return go;
        }
    }
}
