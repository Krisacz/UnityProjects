using UnityEngine;
using System.Collections;
using Assets.Scripts;
using Assets.Scripts.Models;
using Assets.Scripts.Views;
using UnityEngine.EventSystems;

public class InventorySlotController : MonoBehaviour, IPointerDownHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    private Vector2 _dragOffset;
    private Transform _originalParent;
    private bool _droppedOnSlot;
    
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
    }
    
    public void OnDrag(PointerEventData eventData)
    {
        var itemStackView = this.GetComponent<ItemStackView>();
        itemStackView.GetItemGameObject().transform.position = eventData.position + _dragOffset;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if(_droppedOnSlot) return;

        var itemStackView = eventData.pointerDrag.GetComponent<ItemStackView>();
        var itemGameObject = itemStackView.GetItemGameObject();

        //Set current parent to original parent from before drag
        itemGameObject.transform.SetParent(_originalParent);
        
        //Reset position
        itemGameObject.transform.position = _originalParent.position;
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

        //Set local slot to dragged item and dragged slot to local item (the one we dropped it on)
        localItemStackView.UpdateItemStack(draggedItemStack, draggedGameObject);
        draggedItemStackView.UpdateItemStack(localItemStack, localGameObject);

        //Set that we actually droppped dragged item on top of anoter slot
        var draggedInventorySlotController = eventData.pointerDrag.GetComponent<InventorySlotController>();
        draggedInventorySlotController.DroppedOnSlot();
    }

    private void DroppedOnSlot()
    {
        _droppedOnSlot = true;
    }
}
