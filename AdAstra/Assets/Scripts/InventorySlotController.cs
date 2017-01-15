using System;
using UnityEngine;
using Assets.Scripts.Views;
using UnityEngine.EventSystems;

public class InventorySlotController : MonoBehaviour, IPointerDownHandler,
    IDragHandler, IEndDragHandler, IDropHandler
{
    private Vector2 _dragOffset;
    private Transform _originalParent;
    private bool _droppedOnSlot;
    private bool _emptied;
    
    public void OnPointerDown(PointerEventData eventData)
    {
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
    
    public void OnDrag(PointerEventData eventData)
    {
        var itemStackView = this.GetComponent<ItemStackView>();
        if (!itemStackView.HasItem) return;
        itemStackView.GetItemGameObject().transform.position = eventData.position + _dragOffset;
    }

    public void OnDrop(PointerEventData eventData)
    {
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

                    if(localNewCount == 0)
                    {
                        draggedInventorySlotController.Emptied();
                    }
                }
            }
        }

        //Set local slot to dragged item and dragged slot to local item (the one we dropped it on)
        localItemStackView.UpdateItemStack(draggedItemStack, draggedGameObject);
        draggedItemStackView.UpdateItemStack(localItemStack, localGameObject);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        var itemStackView = this.GetComponent<ItemStackView>();
        if (!itemStackView.HasItem) return;

        if (_emptied)
        {
            itemStackView.DeleteExistingItem();
            return;
        }

        if (_droppedOnSlot) return;

        var itemGameObject = itemStackView.GetItemGameObject();

        //Set current parent to original parent from before drag
        itemGameObject.transform.SetParent(_originalParent);
        
        //Reset position
        itemGameObject.transform.position = _originalParent.position;
    }

    private void DroppedOnSlot()
    {
        _droppedOnSlot = true;
    }

    private void Emptied()
    {
        _emptied = true;
    }
}
