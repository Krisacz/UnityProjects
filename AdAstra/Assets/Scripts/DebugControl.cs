using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using Assets.Scripts.Models;

public class DebugControl : MonoBehaviour
{
    public GameObject InventoryController;

    [Header("===== INSERT ITEMS =====")]
    [Space(50)]
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

    }

    // Update is called once per frame
    void Update()
    {
        AddItemsControl();
        ItemsToCheck();
        ItemsToRemove();
    }

    private void AddItemsControl()
    {
        var ic = InventoryController.GetComponent<InventoryController>();
        if (Input.GetKeyDown(KeyCode.Alpha1)) ic.AddItem(ItemId.IronOre, IronToInsert, InventoryToInsert);
        if (Input.GetKeyDown(KeyCode.Alpha2)) ic.AddItem(ItemId.GoldOre, GoldToInsert, InventoryToInsert);
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
