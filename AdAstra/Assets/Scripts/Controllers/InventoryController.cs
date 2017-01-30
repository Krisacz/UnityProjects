using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Db;
using Assets.Scripts.Models;
using Assets.Scripts.Views;
using UnityEngine;

namespace Assets.Scripts.Controllers
{
    public class InventoryController : MonoBehaviour
    {
        public GameObject[] Inventories;

        public void Start()
        {
            AddItem(ItemId.Foundation, 10);
            AddItem(ItemId.Floor, 10);
            AddItem(ItemId.Wall, 10);
            AddItem(ItemId.Welder, 1);
        }

        #region ADD ITEM
        //Returns number of NOT added items due to lack of space - 0 means that all requested items has been added
        public int AddItem(ItemId id, int count, int inventoryIndex = -1)
        {
            if (inventoryIndex <= -1)
            {
                return AddItemToFirstAvailableInventory(id, count);
            }
            else
            {
                return AddItemToSpecificInventory(id, count, inventoryIndex);
            }
        }

        private int AddItemToSpecificInventory(ItemId id, int count, int inventoryIndex)
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

            Log.Info("InventoryController", "AddItemToSpecificInventory",
                string.Format("Inventory [{0}] full - unable to insert {1} x{2}", inventoryIndex, id, countToInsert));
            return countToInsert;
        }

        private int AddItemToFirstAvailableInventory(ItemId id, int count)
        {
            var countToInsert = count;

            for (var index = 0; index < Inventories.Length; index++)
            {
                countToInsert = AddItemToSpecificInventory(id, countToInsert, index);
            }

            if (countToInsert == 0) return 0;

            Log.Info("InventoryController", "AddItemToFirstAvailableInventory",
                string.Format("Inventory(s) full - unable to insert {0} x{1}", id, countToInsert));
            return countToInsert;
        }
        #endregion

        #region CHECK ADD ITEM
        // Returns items/count of NOT added items due to lack of space
        // empty means that all requested items can be be added
        public Dictionary<ItemId, int> CheckAddItem(Dictionary<ItemId, int> items, int inventoryIndex = -1)
        {
            if (inventoryIndex <= -1)
            {
                return CheckAddItemToFirstAvaInv(items);
            }
            else
            {
                return CheckAddItemToSpecInv(items, inventoryIndex);
            }
        }

        private Dictionary<ItemId, int> CheckAddItemToSpecInv(Dictionary<ItemId, int> items,
            int inventoryIndex)
        {
            var toInsert = new Dictionary<ItemId, int>(items);
            var inventory = Inventories[inventoryIndex];
            var allSlots = inventory.GetComponentsInChildren<ItemStackView>();
            foreach (var isv in allSlots)
            {
                var itemStack = isv.GetItemStack();

                if (isv.HasItem)
                {
                    var itemId = itemStack.Item.ItemId;
                    if (toInsert.ContainsKey(itemId))
                    {
                        var itemMaxStack = itemStack.Item.MaxStackSize;
                        var canInsert = itemMaxStack - itemStack.Count;
                        toInsert[itemId] -= Math.Min(canInsert, toInsert[itemId]);
                        if (toInsert[itemId] <= 0) toInsert.Remove(itemId);
                    }
                }
                else
                {
                    var firstItemId = toInsert.First().Key;
                    var firstItemCount = toInsert.First().Value;
                    var item = ItemsDatabase.Get(firstItemId);
                    toInsert[firstItemId] -= Math.Min(firstItemCount, item.MaxStackSize);
                    if (toInsert[firstItemId] == 0) toInsert.Remove(firstItemId);
                }

                //If we've removed all return null
                if (toInsert.Count == 0) return toInsert;
            }

            return toInsert;
        }

        private Dictionary<ItemId, int> CheckAddItemToFirstAvaInv(Dictionary<ItemId, int> items)
        {
            var toInsert = new Dictionary<ItemId, int>(items);

            for (var index = 0; index < Inventories.Length; index++)
            {
                toInsert = CheckAddItemToSpecInv(toInsert, index);
                if (toInsert.Count == 0) return toInsert;
            }

            return toInsert;
        }
        #endregion

        #region CHECK INVENTORIES
        public Dictionary<ItemId, int> Check(Dictionary<ItemId, int> items, int inventoryIndex = -1)
        {
            if (inventoryIndex <= -1)
            {
                return CheckAllInventories(items);
            }
            else
            {
                return CheckSpecificInventory(items, inventoryIndex);
            }
        }

        private Dictionary<ItemId, int> CheckAllInventories(Dictionary<ItemId, int> items)
        {
            var left = items;

            for (var index = 0; index < Inventories.Length; index++)
            {
                left = CheckSpecificInventory(left, index);
            }

            if (left.Count == 0) return left;

            var missingItemsStr = string.Empty;
            foreach (var i in left) missingItemsStr += string.Format("{0} [{1}] ", i.Key, i.Value);
            Log.Info("InventoryController", "CheckInventories", string.Format("Missing items: {0}", missingItemsStr));
            return left;
        }

        private Dictionary<ItemId, int> CheckSpecificInventory(Dictionary<ItemId, int> items, int inventoryIndex)
        {
            var left = items;
            var inventory = Inventories[inventoryIndex];
            var allSlots = inventory.GetComponentsInChildren<ItemStackView>();
            foreach (var isv in allSlots)
            {
                if (!isv.HasItem) continue;
                var stack = isv.GetItemStack();
                if (!left.ContainsKey(stack.Item.ItemId)) continue;
                left[stack.Item.ItemId] -= stack.Count;
                if (left[stack.Item.ItemId] <= 0) left.Remove(stack.Item.ItemId);
            }

            if (left.Count == 0) return left;

            var missingItemsStr = string.Empty;
            foreach (var i in left) missingItemsStr += string.Format("{0} [{1}] ", i.Key, i.Value);
            Log.Info("InventoryController", "CheckSpecificInventory", string.Format("Missing items: {0}", missingItemsStr));
            return left;
        }
        #endregion

        #region REMOVE ITEM
        //Will return number of items NOT removed due to lack of items - 0 means all required items has been removed
        public int RemoveItem(ItemId id, int count, int inventoryIndex = -1)
        {
            if (inventoryIndex <= -1)
            {
                return RemoveItemFromAnyInventory(id, count);
            }
            else
            {
                return RemoveItemFromSpecificInventory(id, count, inventoryIndex);
            }
        }

        private int RemoveItemFromAnyInventory(ItemId id, int count)
        {
            var left = count;

            for (var index = 0; index < Inventories.Length; index++)
            {
                if(left == 0) break;
                left = RemoveItemFromSpecificInventory(id, left, index);
            }

            if (left == 0) return left;

            Log.Info("InventoryController", "RemoveItemFromAnyInventory",
                string.Format("Insufficient item(s) to remove: {0} x{1}", id, left));
            return left;
        }

        private int RemoveItemFromSpecificInventory(ItemId id, int count, int inventoryIndex)
        {
            var left = count;
            var inventory = Inventories[inventoryIndex];
            var allSlots = inventory.GetComponentsInChildren<ItemStackView>();
            foreach (var isv in allSlots)
            {
                if(left == 0) break;
                if (!isv.HasItem) continue;
                var stack = isv.GetItemStack();
                if (stack.Item.ItemId != id) continue;
                var amountToRemove = Math.Min(left, stack.Count);
                left -= amountToRemove;
                isv.UpdateStackCount(amountToRemove * -1);
            }

            if (left == 0) return left;
            
            Log.Info("InventoryController", "RemoveItemFromSpecificInventory",
                string.Format("Insufficient item(s) to remove [Inventory={0}]: {1} x{2}", inventoryIndex, id, left));
            return left;
        }
        #endregion

        #region CHECK INVENTORIES
        public int GetCount(ItemId itemId, int inventoryIndex = -1)
        {
            if (inventoryIndex <= -1)
            {
                return GetFromAllInventories(itemId);
            }
            else
            {
                return GetFromSpecificInventory(itemId, 0, inventoryIndex);
            }
        }

        private int GetFromAllInventories(ItemId itemId)
        {
            var total = 0;

            for (var index = 0; index < Inventories.Length; index++)
            {
                total = GetFromSpecificInventory(itemId, total, index);
            }
            return total;
        }

        private int GetFromSpecificInventory(ItemId itemId, int current, int inventoryIndex)
        {
            var total = current;
            var inventory = Inventories[inventoryIndex];
            var allSlots = inventory.GetComponentsInChildren<ItemStackView>();
            foreach (var isv in allSlots)
            {
                if (!isv.HasItem) continue;
                var stack = isv.GetItemStack();
                if (stack.Item.ItemId == itemId) total += stack.Count;
            }
            return total;
        }
        #endregion

        #region FREE SLOTS
        public int FreeSlots(int inventoryIndex = -1)
        {
            if (inventoryIndex <= -1)
            {
                return FreeSlotsInAllInventories();
            }
            else
            {
                return FreeSlotsISpecificInventory(0, inventoryIndex);
            }
        }

        private int FreeSlotsInAllInventories()
        {
            var total = 0;
            for (var index = 0; index < Inventories.Length; index++)
            {
                total = FreeSlotsISpecificInventory(total, index);
            }
            return total;
        }

        private int FreeSlotsISpecificInventory(int current, int inventoryIndex)
        {
            var total = current;
            var inventory = Inventories[inventoryIndex];
            var allSlots = inventory.GetComponentsInChildren<ItemStackView>();
            foreach (var isv in allSlots) if (!isv.HasItem) total++;
            return total;
        }
        #endregion
    }
}
