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

        public bool InventoryAddItem(Item item)
        {
            return GameObjectAddItem(Inventory, item);
        }

        private static bool GameObjectAddItem(GameObject inventory, Item item)
        {
            var allChildren = inventory.GetComponentsInChildren<ItemStackView>();

            //TODO Do proper item addition etc - search for empty slot, search for same item type, refill stacks, reject if no space etc
            //foreach (var child in allChildren)
            //{
            //    // do what you want with the transform
            //}
            //var slot = allChildren.First();
            //slot.ItemStack.Item = item;
            //slot.ItemStack.Count = 1;
            //item.GameObject.transform.parent = slot.transform;
            //item.GameObject.SetActive(false);
            //var image = slot.GetComponent<Image>();
            //image.sprite = item.GameObject.GetComponent<SpriteRenderer>().sprite;
            //image.color = new Color(1f, 1f, 1f, 1f);

            return true;
        }

        // Use this for initialization
        void Start () {
	
        }
	
        // Update is called once per frame
        void Update () {
	
        }
    }
}
