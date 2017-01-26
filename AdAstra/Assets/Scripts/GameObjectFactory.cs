using Assets.Scripts.Controllers;
using Assets.Scripts.Db;
using Assets.Scripts.Models;
using Assets.Scripts.Views;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace Assets.Scripts
{
    public static class GameObjectFactory
    {
        #region BLUEPRINTS STUFF
        public static void BlueprintGroup(BlueprintGroup group, Transform parent)
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

        public static GameObject ItemBlueprint(Blueprint blueprint, Transform parent, bool canBeSelected = true)
        {
            var go = FromPrefab("ItemBlueprint");

            var spriteName = ItemsDatabase.Get(blueprint.ResultItemId).SpriteName;
            go.GetComponent<Image>().sprite = SpritesDatabase.Get(spriteName);
            var itemBlueprintController = go.GetComponent<ItemBlueprintController>();
            itemBlueprintController.SetBlueprint(blueprint);
            itemBlueprintController.CanBeSelected = canBeSelected;
            go.GetComponent<Button>().enabled = canBeSelected;
            go.transform.SetParent(parent);

            //Set name for debuging
            var gameObjectName = string.Format("ItemBlueprint [{0}]", blueprint.ResultItemId);
            go.name = gameObjectName;

            return go;
        }

        public static GameObject BlueprintRequiredItem(ItemId itemId, int count, Transform parent)
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
        #endregion

        #region ITEM STUFF
        public static GameObject Item(Item item)
        {
            var go = FromPrefab("Item");

            //Set name for debuging
            var gameObjectName = string.Format("Item ID[{0}]", item.ItemId);
            go.name = gameObjectName;

            //Return game object
            return go;
        }
        #endregion

        #region ESCAPE POD
        public static GameObject EscapePod()
        {
            var go = FromPrefab("EscapePod");

            //Set name for debuging
            var gameObjectName = "Escape Pod";
            go.name = gameObjectName;

            return go;
        }
        #endregion

        #region PLAYER 
        public static GameObject Player(int x, int y)
        {
            var go = FromPrefab("Player");
            go.transform.position = new Vector3(x, y, go.transform.position.z);

            //Set name for debuging
            var gameObjectName = "Player";
            go.name = gameObjectName;

            return go;
        }
        #endregion

        #region STRUCTURE
        public static GameObject StructureSlot(int x, int y, Transform parent)
        {
            var go = FromPrefab("StructureSlot");
            go.name = string.Format("StructureSlot X[{0}]-Y[{1}]", x, y);
            go.transform.position = new Vector3(x, y);
            go.transform.parent = parent;
            go.GetComponent<StructureSlotView>().SetGridPosition(x, y);
            return go;
        }

        public static GameObject StructureItem(Item item, bool blocking, 
            StructureElevation elevation, Transform transform)
        {
            var go = FromPrefab(blocking ? "StructureBlocking" : "StructureNonBlocking");

            //Set name for debuging
            var gameObjectName = string.Format("Structure ID[{0}]", item.ItemId);
            go.name = gameObjectName;

            go.transform.position = transform.position;
            go.transform.parent = transform;
            var spriteRenderer = go.GetComponent<SpriteRenderer>();
            spriteRenderer.sprite = SpritesDatabase.Get(item.SpriteName);
            spriteRenderer.sortingOrder = (int)elevation;
            return go;
        }
        #endregion

        #region HELP METHODS
        private static GameObject FromPrefab(string prefabName)
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
        #endregion
    }
}
