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
        
        #region SET NEW ITEM STACK
        public void SetNewItemStack(ItemId itemId, int count)
        {
            var item = ItemsDatabase.Get(itemId);
            var go = GameObjectFactory.FromItemId(itemId);

            _itemStack = new ItemStack();
            _itemStack.Count = count;
            _itemStack.Item = item;
            _itemGameObject = go;
            _itemGameObject.transform.SetParent(this.transform.FindChild("ItemPanel"));
            _itemGameObject.transform.position = this.transform.position;
            var goImage = go.GetComponent<Image>();
            goImage.sprite = SpritesDatabase.Get(item.SpriteName);
            SetStackCountText(_itemStack.Count);

            HasItem = true;
        }
        #endregion

        #region UpdateItemStack
        public void UpdateItemStack(ItemStack itemStack, GameObject go)
        {
            if (itemStack == null || go == null)
            {
                EmptyItemStackView(false);
                return;
            }

            _itemStack = itemStack;
            _itemGameObject = go;
            _itemGameObject.transform.SetParent(this.transform.FindChild("ItemPanel"));
            _itemGameObject.transform.position = this.transform.position;
            var goImage = go.GetComponent<Image>();
            goImage.sprite = SpritesDatabase.Get(_itemStack.Item.SpriteName);
            SetStackCountText(_itemStack.Count);

            HasItem = true;
        }
        #endregion

        public void UpdateStackCount(int count)
        {
            _itemStack.Count += count;

            if (_itemStack.Count < 0)
                Log.Error("ItemStackView", "UpdateStackCount", "Can not change stack count below 0!");

            if (_itemStack.Count > 0)
            {
                SetStackCountText(_itemStack.Count);
                return;
            }

            //If count == 0 delete item
            EmptyItemStackView(true);
        }
        
        public ItemStack GetItemStack()
        {
            return _itemStack;
        }

        public GameObject GetItemGameObject()
        {
            return _itemGameObject;
        }

        public void EmptyItemStackView(bool deleteGameObject)
        {
            if (deleteGameObject && _itemGameObject != null)
            {
                Destroy(_itemGameObject);
            }
            else
            {
                _itemGameObject = null;
            }

            _itemStack = null;
            HasItem = false;
            SetStackCountText(null);
        }

        private void SetStackCountText(int? count)
        {
            var c = count.HasValue && count.Value > 0 ? count.Value.ToString() : string.Empty;
            if(_itemGameObject == null) return;
            var countTextGo = _itemGameObject.transform.FindChild("StackCountText");
            countTextGo.GetComponent<Text>().text = c;
        }
    }
}
