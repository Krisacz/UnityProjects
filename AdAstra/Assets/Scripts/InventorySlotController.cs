using UnityEngine;
using System.Collections;
using Assets.Scripts;
using Assets.Scripts.Views;
using UnityEngine.EventSystems;

public class InventorySlotController : MonoBehaviour, IPointerDownHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    private Vector2 _dragOffset;
    private Transform _originalParent;

    public void OnPointerDown(PointerEventData eventData)
    {
        var itemStackView = this.GetComponent<ItemStackView>();
        if (!itemStackView.HasItem) return;
        var itemGameObject = itemStackView.GetItemGameObject();
        _originalParent = this.transform.parent;
        itemGameObject.transform.SetParent(itemGameObject.transform.root);
        _dragOffset = itemGameObject.transform.position - (Vector3)eventData.position;
        itemGameObject.transform.position = eventData.position + _dragOffset;
    }
    
    public void OnDrag(PointerEventData eventData)
    {
        var itemStackView = this.GetComponent<ItemStackView>();
        itemStackView.GetItemGameObject().transform.position = eventData.position + _dragOffset;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        //TODO dropping outside slots
        Log.Info("##### OnEndDrag #####");
        //this.transform.SetParent(_originalParent.transform);
        //this.transform.position = _originalParent.transform.position;
        //_originalParent = null;
    }

    public void OnDrop(PointerEventData eventData)
    {
        Log.Info("##### OnDrop #####");

        var localItemStackView = this.GetComponent<ItemStackView>();
        var draggedItemStackView = eventData.pointerDrag.GetComponent<ItemStackView>();

        if (localItemStackView.HasItem)
        {
            var localItemStack = localItemStackView.GetItemStack();
            var localGameObject = localItemStackView.GetItemGameObject();
            var draggedItemStack = draggedItemStackView.GetItemStack();
            var draggedGameObject = draggedItemStackView.GetItemGameObject();

            localItemStackView.UpdateItemStack(draggedItemStack, draggedGameObject);
            draggedItemStackView.UpdateItemStack(localItemStack, localGameObject);
        }
        else
        {
            var draggedItemStack = draggedItemStackView.GetItemStack();
            var draggedGameObject = draggedItemStackView.GetItemGameObject();

            localItemStackView.UpdateItemStack(draggedItemStack, draggedGameObject);
            draggedItemStackView.EmptyItemStackView();
        }
    }
}
