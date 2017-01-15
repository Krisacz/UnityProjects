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
            SetStackCount(_itemStack.Count);

            HasItem = true;
        }
        #endregion

        #region UpdateItemStack
        public void UpdateItemStack(ItemStack itemStack, GameObject go)
        {
            if (itemStack == null || go == null)
            {
                EmptyItemStackView();
                return;
            }

            _itemStack = itemStack;
            _itemGameObject = go;
            _itemGameObject.transform.SetParent(this.transform.FindChild("ItemPanel"));
            _itemGameObject.transform.position = this.transform.position;
            var goImage = go.GetComponent<Image>();
            goImage.sprite = SpritesDatabase.Get(_itemStack.Item.SpriteName);
            SetStackCount(_itemStack.Count);

            HasItem = true;
        }
        #endregion

        public void UpdateStackCount(int count)
        {
            _itemStack.Count += count;
            if (_itemStack.Count < 0)
                Log.Error("ItemStackView", "UpdateStackCount", "Can not change stack count below 0!");
            if(_itemStack.Count == 0)
                EmptyItemStackView();

            //Update stack count text
            SetStackCount(_itemStack.Count);
        }

        public void DeleteExistingItem()
        {
            if (_itemStack == null || _itemGameObject == null) return;
            _itemStack = null;
            Destroy(_itemGameObject);
            HasItem = false;
            SetStackCount(-1);
        }

        public ItemStack GetItemStack()
        {
            return _itemStack;
        }

        public GameObject GetItemGameObject()
        {
            return _itemGameObject;
        }

        private void EmptyItemStackView()
        {
            _itemStack = null;
            _itemGameObject = null;
            HasItem = false;
            SetStackCount(-1);
        }

        private void SetStackCount(int? count)
        {
            var c = count.HasValue && count.Value > 0 ? count.Value.ToString() : string.Empty;
            if(_itemGameObject == null) return;
            var countTextGo = _itemGameObject.transform.FindChild("StackCountText");
            countTextGo.GetComponent<Text>().text = c;
        }
    }
}
