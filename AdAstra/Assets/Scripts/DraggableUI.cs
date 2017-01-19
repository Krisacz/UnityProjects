using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Scripts
{
    public class DraggableUI : MonoBehaviour, IPointerDownHandler, IDragHandler, IEndDragHandler
    {
        private Vector2 _dragOffset;
        public GameObject GameObjectToMove;

        public void OnPointerDown(PointerEventData eventData)
        {
            //Set offset to the point where we are actually "touching" the item
            _dragOffset = GameObjectToMove.transform.position - (Vector3)eventData.position;
            GameObjectToMove.transform.position = eventData.position + _dragOffset;
        }

        public void OnDrag(PointerEventData eventData)
        {
            GameObjectToMove.transform.position = eventData.position + _dragOffset;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            //TODO Keep draggable object within screen boundries
            //var screenW = Screen.width;
            //var screenH = Screen.height;
            //var d = GameObjectToMove.transform.position;
            //var pos = RectTransformUtility.WorldToScreenPoint(null, d);
            //Log.Info(pos.x + " - " + pos.y);
        }
    }
}
