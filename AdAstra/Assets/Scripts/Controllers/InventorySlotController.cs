using System;
using System.Security.Policy;
using Assets.Scripts.Db;
using Assets.Scripts.Views;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Scripts.Controllers
{
    public class InventorySlotController : TooltipController, IPointerDownHandler,
        IDragHandler, IEndDragHandler, IDropHandler, IPointerClickHandler
    {
        //TODO Do caching of different objects here - like this.GetComponent<ItemStackView>();
        //TODO Will improve perf 
         
        private Vector2 _dragOffset;
        private Transform _originalParent;
        private bool _droppedOnSlot;
        private bool _emptied;
        public bool CanReceiveItem = true;
        public bool CanTakeItemFrom = true;
        public bool CanBeSelected = false;

        #region ON POINTER DOWN
        public void OnPointerDown(PointerEventData eventData)
        {
            if(!CanTakeItemFrom) return;
            ShowTooltip = false;

            var itemStackView = this.GetComponent<ItemStackView>();
            if (!itemStackView.HasItem) return;
            var itemGameObject = itemStackView.GetItemGameObject();

            //Get original parent to be able to return to original position
            _originalParent = itemGameObject.transform.parent;

            //Set current parent to root so it is visible above all other slots/items etc
            itemGameObject.transform.SetParent(itemGameObject.transform.root);

            //Set offset to the point where we are actually "touching" the item
            _dragOffset = itemGameObject.transform.position - (Vector3)eventData.position;
            itemGameObject.transform.position = eventData.position + _dragOffset;

            //Assume that we will drop item not on the other slot
            _droppedOnSlot = false;

            _emptied = false;
        }
        #endregion

        #region ON DRAG
        public void OnDrag(PointerEventData eventData)
        {
            if (!CanTakeItemFrom) return;

            var itemStackView = this.GetComponent<ItemStackView>();
            if (!itemStackView.HasItem) return;
            itemStackView.GetItemGameObject().transform.position = eventData.position + _dragOffset;
        }
        #endregion

        #region ON DROP
        public void OnDrop(PointerEventData eventData)
        {
            if (!CanReceiveItem) return;
            
            var localItemStackView = this.GetComponent<ItemStackView>();
            var draggedItemStackView = eventData.pointerDrag.GetComponent<ItemStackView>();

            //Get local item/go - can be null!
            var localItemStack = localItemStackView.GetItemStack();
            var localGameObject = localItemStackView.GetItemGameObject();

            //Get dragged item/go
            var draggedItemStack = draggedItemStackView.GetItemStack();
            var draggedGameObject = draggedItemStackView.GetItemGameObject();

            //Set that we actually droppped dragged item on top of anoter slot
            var draggedInventorySlotController = eventData.pointerDrag.GetComponent<InventorySlotController>();
            draggedInventorySlotController.DroppedOnSlot();

            //Modify stacks if there are of the same type (and there is a room to change count)
            if (localItemStack != null)
            {
                if (draggedItemStack.Item.ItemId == localItemStack.Item.ItemId)
                {
                    var maxStackSize = draggedItemStack.Item.MaxStackSize;
                    if (draggedItemStack.Count != maxStackSize && localItemStack.Count != maxStackSize)
                    {
                        var addToDragged = Math.Min(maxStackSize - draggedItemStack.Count, localItemStack.Count);
                        var darggedNewCount = draggedItemStack.Count + addToDragged;
                        draggedItemStack.Count = darggedNewCount;

                        var localNewCount = localItemStack.Count - addToDragged;
                        localItemStack.Count = localNewCount;

                        if (localNewCount == 0)
                        {
                            draggedInventorySlotController.Emptied();
                        }
                    }
                }
            }

            //Set local slot to dragged item and dragged slot to local item (the one we dropped it on)
            localItemStackView.UpdateItemStack(draggedItemStack, draggedGameObject);
            draggedItemStackView.UpdateItemStack(localItemStack, localGameObject);

            if (CanBeSelected)
            {
                ItemSelectionController.UpdateSelection(this.transform.GetSiblingIndex() - 1);
            }
        }
        #endregion

        #region ON END DRAG
        public void OnEndDrag(PointerEventData eventData)
        {
            ShowTooltip = true;
            
            var itemStackView = this.GetComponent<ItemStackView>();
            if (!itemStackView.HasItem) return;

            if (_emptied)
            {
                itemStackView.EmptyItemStackView(true);
                return;
            }

            if (_droppedOnSlot) return;

            var itemGameObject = itemStackView.GetItemGameObject();

            //Set current parent to original parent from before drag
            itemGameObject.transform.SetParent(_originalParent);

            //Reset position
            itemGameObject.transform.position = _originalParent.position;
        }
        #endregion

        #region ON POINTER CLICK
        public void OnPointerClick(PointerEventData eventData)
        {
            if (CanBeSelected)
            {
                ItemSelectionController.UpdateSelection(this.transform.GetSiblingIndex() - 1);
            }

            this.OnEndDrag(eventData);
        }
        #endregion

        #region FLAGS
        private void DroppedOnSlot()
        {
            _droppedOnSlot = true;
        }

        private void Emptied()
        {
            _emptied = true;
        }
        #endregion

        #region TOOLTIP
        public override string GetTooltipTitle()
        {
            var isv = this.GetComponent<ItemStackView>();
            if (isv == null || !isv.HasItem) return null;
            var i = isv.GetItemStack();
            if (i == null) return null;
            var title = i.Item.Title;
            return title;
        }

        public override string GetTooltipDescription()
        {
            var isv = this.GetComponent<ItemStackView>();
            if (isv == null || !isv.HasItem) return null;
            var i = isv.GetItemStack();
            if (i == null) return null;
            var description = i.Item.Description;
            return description;
        }
        #endregion
    }
}
