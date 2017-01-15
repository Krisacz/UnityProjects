using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Models;
using Assets.Scripts.Views;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class InventoryController : MonoBehaviour
    {
        public GameObject[] Inventories;

        public int AddItem(ItemId id, int count, int inventoryIndex = -1)
        {
            if (inventoryIndex <= -1)
            {
                return AddToFirstAvailableInventory(id, count);
            }
            else
            {
                return AddToSpecificInventory(id, count, inventoryIndex);
            }
        }

        private int AddToSpecificInventory(ItemId id, int count, int inventoryIndex)
        {
            var countToInsert = count;
            var inventory = Inventories[inventoryIndex];
            var allSlots = inventory.GetComponentsInChildren<ItemStackView>();
            foreach (var isv in allSlots)
            {
                if (!isv.HasItem)
                {
                    var itemMaxStack = ItemsDatabase.Get(id).MaxStackSize;
                    var willInsertCount = Math.Min(countToInsert, itemMaxStack);
                    countToInsert = countToInsert - willInsertCount;
                    if (willInsertCount > 0) isv.SetNewItemStack(id, willInsertCount);
                    if (countToInsert == 0) return 0;
                }
                else
                {
                    var slotItem = isv.GetItemStack();
                    if (slotItem == null || isv.GetItemStack().Item.ItemId != id) continue;
                    var freeStackSpace = slotItem.Item.MaxStackSize - slotItem.Count;
                    var willInsertCount = Math.Min(countToInsert, freeStackSpace);
                    countToInsert = countToInsert - willInsertCount;
                    if (willInsertCount > 0) isv.UpdateStackCount(willInsertCount);
                    if (countToInsert == 0) return 0;
                }
            }

            Log.Info("InventoryController", "AddToSpecificInventory",
                string.Format("Inventory [{0}] full - unable to insert {1} x{2}", inventoryIndex, id, countToInsert));
            return countToInsert;
        }

        //Will return number of items NOT added due to lack of space - 0 means added all requested items
        private int AddToFirstAvailableInventory(ItemId id, int count)
        {
            var countToInsert = count;

            for (var index = 0; index < Inventories.Length; index++)
            {
                countToInsert = AddToSpecificInventory(id, countToInsert, index);
            }

            if (countToInsert == 0) return 0;

            Log.Info("InventoryController", "AddToFirstAvailableInventory",
                string.Format("Inventory(s) full - unable to insert {0} x{1}", id, countToInsert));
            return countToInsert;
        }

        public Dictionary<ItemId, int> CheckInventory(Dictionary<ItemId, int> items, int inventoryIndex)
        {
            //TODO loop through inventories (or one inventory specified) and "gather" required items and their count
            //TODO Return anything what's missing
            throw new NotImplementedException();
        }

        //Will return number of items NOT removed due to lack of items - 0 means all required items has been removed
        //TODO You should not call remove before checking if that item and count exist in the inventory
        //TODO unless there is a need of a partial removal (e.i. quest deliver could possibly be partial)
        public int RemoveItem(ItemId id, int count, int inventoryIndex = -1)
        {
            throw new NotImplementedException();
        }
    }
}
