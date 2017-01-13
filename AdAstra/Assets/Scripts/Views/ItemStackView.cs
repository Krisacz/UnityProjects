using System;
using Assets.Scripts.Models;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Views
{
    public class ItemStackView : MonoBehaviour
    {
        private ItemStack _itemStack;
        private GameObject _itemGameObject;
        public bool HasItem { get; private set; }

        public void SetNewItemStack(int amount, ItemId itemId)
        {
            var item = ItemsDatabase.Get(itemId);
            var go = GameObjectFactory.FromItemId(itemId);

            _itemStack = new ItemStack();
            _itemStack.Count = amount;
            _itemStack.Item = item;
            _itemGameObject = go;
            _itemGameObject.transform.parent = this.transform.GetChild(0);
            _itemGameObject.transform.position = this.transform.position;
            var goImage = go.GetComponent<Image>();
            goImage.sprite = SpritesDatabase.Get(item.SpriteName);

            HasItem = true;
        }

        public void UpdateItemStack(ItemStack itemStack, GameObject go)
        {
            if (itemStack == null || go == null)
            {
                EmptyItemStackView();
                return;
            }

            _itemStack = itemStack;
            _itemGameObject = go;
            _itemGameObject.transform.parent = this.transform.GetChild(0);
            _itemGameObject.transform.position = this.transform.position;
            var goImage = go.GetComponent<Image>();
            goImage.sprite = SpritesDatabase.Get(_itemStack.Item.SpriteName);

            HasItem = true;
        }

        private void EmptyItemStackView()
        {
            _itemStack = null;
            _itemGameObject = null;
            HasItem = false;    
        }
        
        public ItemStack GetItemStack()
        {
            return _itemStack;
        }

        public GameObject GetItemGameObject()
        {
            return _itemGameObject;
        }
    }
}
