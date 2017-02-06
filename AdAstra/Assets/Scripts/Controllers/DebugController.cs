using System.Collections.Generic;
using Assets.Scripts.Models;
using UnityEngine;

namespace Assets.Scripts.Controllers
{
    //[ExecuteInEditMode]
    public class DebugController : MonoBehaviour
    {
        public static bool InfiniteItems = true;

        public GameObject InventoryController;
        public GameObject CraftingController;

        [Header("===== INSERT ITEMS =====")]
        public int IronToInsert;
        public int GoldToInsert;
        public int InventoryToInsert = -1;

        [Header("===== CHECK ITEMS =====")]
        public int IronToCheck;
        public int GoldToCheck;

        [Header("===== REMOVE ITEMS =====")]
        public ItemId ItemToRemove;
        public int RemoveCount;
        public int InventoryToRemoveFrom = -1;
    

        // Use this for initialization
        void Start()
        {
            if (InfiniteItems)
            {
                Log.Warn("DebugController", "Start", "Warning! Infinite items are ENABLED!");
            } 
        }

        // Update is called once per frame
        void Update()
        {
            AddItemsControl();
            ItemsToCheck();
            ItemsToRemove();
            NotificationsControl();
        }

        private void NotificationsControl()
        {
            //if (Input.GetKeyDown(KeyCode.Alpha5)) NotificationFeedController.Add("iron_ore", "+1 Raw Iron Ore");
            //if (Input.GetKeyDown(KeyCode.Alpha6)) NotificationFeedController.Add("gold_ore", "+1 Raw Gold Ore");
            //if (Input.GetKeyDown(KeyCode.Alpha7)) NotificationFeedController.Add(string.Empty, "Some test just-message no-image.");
            //if (Input.GetKeyDown(KeyCode.Alpha8)) NotificationFeedController.Add(Icons.Tick, "Some test success message");
            //if (Input.GetKeyDown(KeyCode.Alpha9)) NotificationFeedController.Add(Icons.Warning, "Some test warning message");
            //if (Input.GetKeyDown(KeyCode.Alpha0)) NotificationFeedController.Add(Icons.Error, "Some test error message");
        }

        private void AddItemsControl()
        {
            var ic = InventoryController.GetComponent<InventoryController>();
            if (Input.GetKeyDown(KeyCode.Keypad1))
            {
                ic.AddItem(ItemId.IronOre, IronToInsert, InventoryToInsert);
                CraftingController.GetComponent<CraftingController>().UpdateSelectedBlueprintRequirements();
            }

            if (Input.GetKeyDown(KeyCode.Keypad2))
            {
                ic.AddItem(ItemId.GoldOre, GoldToInsert, InventoryToInsert);
                CraftingController.GetComponent<CraftingController>().UpdateSelectedBlueprintRequirements();
            }
        }

        private void ItemsToCheck()
        {
            var ic = InventoryController.GetComponent<InventoryController>();
            var d = new Dictionary<ItemId, int> { { ItemId.IronOre, IronToCheck }, { ItemId.GoldOre, GoldToCheck }};
            if (Input.GetKeyDown(KeyCode.I)) Log.Info(ic.Check(d).Count    == 0 ? "NULL" : "NOT NULL"); //All inv.
            if (Input.GetKeyDown(KeyCode.O)) Log.Info(ic.Check(d, 0).Count == 0 ? "NULL" : "NOT NULL"); //Hotbar
            if (Input.GetKeyDown(KeyCode.P)) Log.Info(ic.Check(d, 1).Count == 0 ? "NULL" : "NOT NULL"); //Inventory
        }

        private void ItemsToRemove()
        {
            var ic = InventoryController.GetComponent<InventoryController>();
            if (Input.GetKeyDown(KeyCode.X)) ic.RemoveItem(ItemToRemove, RemoveCount, InventoryToRemoveFrom);
        }
    }
}
