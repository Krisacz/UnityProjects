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
        #region ASTEROIDS CONTROLLER
        public static void AsteroidsController()
        {
            FromPrefab("AsteroidsController");
        }
        #endregion

        #region BACKGROUND
        public static void Background()
        {
            FromPrefab("Background");
        }
        #endregion

        #region UI ELEMENTS
        public static GameObject WorkProgressUI(bool visible)
        {
            var uiContainer = GameObject.Find("UIContainer");
            var go = FromPrefab("WorkProgressUI");
            go.name = "WorkProgressUI";
            go.transform.SetParent(uiContainer.transform);
            go.SetActive(visible);
            return go;
        }

        public static GameObject TooltipUI(bool visible)
        {
            var uiContainer = GameObject.Find("UIContainer");
            var go = FromPrefab("TooltipUI");
            go.name = "TooltipUI";
            go.transform.SetParent(uiContainer.transform);
            go.SetActive(visible);
            return go;
        }

        public static GameObject NotificationFeedUI()
        {
            var uiContainer = GameObject.Find("UIContainer");
            var go = FromPrefab("NotificationFeedUI");
            go.name = "NotificationFeedUI";
            go.transform.SetParent(uiContainer.transform);
            return go;
        }

        public static GameObject StatsUI(bool visible)
        {
            var uiContainer = GameObject.Find("UIContainer");
            var go = FromPrefab("StatsUI");
            go.name = "StatsUI";
            go.transform.SetParent(uiContainer.transform);
            go.SetActive(visible);
            return go;
        }

        public static GameObject InventoryBarUI(int initialSlots, bool visible)
        {
            var uiContainer = GameObject.Find("UIContainer");

            var go = FromPrefab("InventoryBarUI");
            var ib = go.transform.FindChild("InventoryBar");
            for (var i = 1; i <= initialSlots; i++)
            {
                var inventorySlotPanel = FromPrefab("InventorySlotPanel");
                inventorySlotPanel.name = string.Format("InventorySlotPanel_{0}", i);
                inventorySlotPanel.transform.SetParent(ib.transform);
            }

            go.name = "InventoryBarUI";
            go.transform.SetParent(uiContainer.transform);

            go.transform.position = Vector3.zero;
            go.transform.localPosition = Vector3.zero;
            

            go.SetActive(visible);
            return go;
        }

        public static GameObject InventoryUI(int initialSlots, bool visible)
        {
            var uiContainer = GameObject.Find("UIContainer");

            var go = FromPrefab("InventoryUI");
            var slotsPanel = go.transform.FindChild("Inventory").transform.FindChild("SlotsPanel");
            for (var i = 1; i <= initialSlots; i++)
            {
                var inventorySlotPanel = FromPrefab("InventorySlotPanel");
                inventorySlotPanel.name = string.Format("InventorySlotPanel_{0}", i);
                inventorySlotPanel.transform.SetParent(slotsPanel);
            }
            go.name = "InventoryUI";
            go.transform.SetParent(uiContainer.transform);
            go.SetActive(visible);
            return go;
        }
        #endregion

        #region CRAFTING
        public static GameObject InteractUI(InteractUIType uiType, int x, int y)
        {
            if (uiType == InteractUIType.None) return null;
            var uiContainerInteract = GameObject.Find("UIContainer").transform.FindChild("Interact");
            var go = FromPrefab("InteractUI");
            var cc = go.GetComponent<CraftingController>();
            cc.Init("TEST TO-DO", "TEST2 TO-DO", uiType);
            go.transform.position = new Vector3(0.5f, 0.5f, 0f);
            go.name = string.Format("InteractUI X[{0}]-Y[{1}]", x, y);
            go.transform.SetParent(uiContainerInteract.transform);
            go.transform.localPosition = Vector3.zero;
            go.SetActive(false);
            return go;
        }
        #endregion

        #region BLUEPRINTS STUFF
        public static void BlueprintGroup(string groupName, Transform parent)
        {
            var go = FromPrefab("BlueprintGroup");

            var spriteName = string.Format("{0}_blueprint_group", groupName.ToLower());
            go.GetComponent<Image>().sprite = SpritesDatabase.Get(spriteName);
            go.transform.FindChild("GroupNameText").GetComponent<Text>().text = groupName.UppercaseFirst();
            go.GetComponent<BlueprintGroupController>().SetBlueprintGroup(groupName);
            go.transform.SetParent(parent);

            //Set name for debuging
            var gameObjectName = string.Format("BlueprintGroup [{0}]", groupName);
            go.name = gameObjectName;
        }

        public static GameObject ItemBlueprint(Blueprint blueprint, Transform parent,
            ItemBlueprintController.BlueprintOnClick actionOnClick)
        {
            var go = FromPrefab("ItemBlueprint");

            var spriteName = ItemsDatabase.Get(blueprint.ResultItemId).SpriteName;
            go.GetComponent<Image>().sprite = SpritesDatabase.Get(spriteName);
            var itemBlueprintController = go.GetComponent<ItemBlueprintController>();
            itemBlueprintController.SetBlueprint(blueprint);
            itemBlueprintController.ActionOnClick = actionOnClick;
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

            go.transform.position =  new Vector3(transform.position.x, transform.position.y, 0f);
            go.transform.parent = transform;
            var spriteRenderer = go.GetComponent<SpriteRenderer>();
            spriteRenderer.sprite = SpritesDatabase.Get(item.SpriteName);
            spriteRenderer.sortingOrder = (int)elevation;
            return go;
        }
        #endregion

        #region ASTEROID
        public static GameObject Asteroid(int spriteId, float scale, Transform parent)
        {
            var go = FromPrefab("Asteroid");
            var sprite = SpritesDatabase.Get("asteroids", spriteId);
            go.GetComponent<SpriteRenderer>().sprite = sprite;
            go.transform.localScale = new Vector3(scale, scale, 1f);
            go.AddComponent<PolygonCollider2D>();
            go.transform.SetParent(parent);
            go.name = "Asteroid";
            return go;
        }
        #endregion

        #region ORE SCAN EFFECT
        public static GameObject OreScanEffect()
        {
            var go = FromPrefab("OreScanEffect");
            go.SetActive(false);
            return go;
        }
        #endregion

        #region ORE NODE
        public static GameObject OreNode(Transform asteroidParent)
        {
            var go = FromPrefab("OreNode");
            go.GetComponent<NodeController>().Init();
            go.transform.SetParent(asteroidParent);
            go.name = "OreNode";
            return go;
        }
        #endregion

        #region NOTIFICATION
        //TODO move to directly to notification controller?
        public static GameObject Noticifaction(string spriteName, string message, Transform parent)
        {
            var go = FromPrefab("Notification");
            var text = go.GetComponentInChildren<Text>();
            text.text = message;
            text.color = new Color(1f, 1f, 1f, 0f);

            if (!string.IsNullOrEmpty(spriteName))
            {
                var image = go.GetComponentInChildren<Image>();
                image.color = new Color(1f, 1f, 1f, 0f);
                image.sprite = SpritesDatabase.Get(spriteName);
            }

            go.transform.SetParent(parent);
            go.GetComponent<RectTransform>().localPosition = new Vector3(0f, -30f, 0f);
            
            //Set name for debuging
            var gameObjectName = "Notification Message";
            go.name = gameObjectName;

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
