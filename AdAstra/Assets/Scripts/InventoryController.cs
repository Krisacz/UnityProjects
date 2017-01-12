using System.Linq;
using Assets.Scripts.Models;
using Assets.Scripts.Views;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class InventoryController : MonoBehaviour
    {
        public GameObject Inventory;
        public GameObject InventoryBar;
        
        // Use this for initialization
        void Start ()
        {
            var allChildren = Inventory.GetComponentsInChildren<ItemStackView>();
            allChildren[1].SetNewItemStack(1, ItemId.GoldOre);
            allChildren[6].SetNewItemStack(1, ItemId.IronOre);
            allChildren[12].SetNewItemStack(1, ItemId.GoldOre);
        }
	
        // Update is called once per frame
        void Update () {
	
        }
    }
}
