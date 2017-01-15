using UnityEngine;
using System.Collections;
using Assets.Scripts;
using Assets.Scripts.Models;

public class DebugControl : MonoBehaviour
{
    public GameObject InventoryController;
    public int IronToInsert;
    public int GoldToInsert;
    public int InventoryToInsert;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        AddItemsControl();
    }

    private void AddItemsControl()
    {
        var ic = InventoryController.GetComponent<InventoryController>();
        if (Input.GetKeyDown(KeyCode.Alpha1)) ic.AddItem(ItemId.IronOre, IronToInsert, InventoryToInsert);
        if (Input.GetKeyDown(KeyCode.Alpha2)) ic.AddItem(ItemId.GoldOre, GoldToInsert, InventoryToInsert);
    }
}
