using System.Collections.Generic;
using Assets.Scripts.Db;
using Assets.Scripts.Models;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Scripts.Views
{
    public class StructureSlotView : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
    {
        private readonly Dictionary<StructureElevation, Item> _items = new Dictionary<StructureElevation, Item>();
        private readonly Dictionary<StructureElevation, GameObject> _gameObjects = new Dictionary<StructureElevation, GameObject>();
        
        //Returns TRUE is succesfully added, false if it already has a structure on this elevation
        public bool AddStructure(StructureElevation elevation, ItemId itemId)
        {
            //Item (structure) already exist
            if (_items.ContainsKey(elevation) && _items[elevation] != null)
            {
                Log.Warn("StructureSlotView", "AddStructure", 
                    string.Format("Structure already exist on Elevation = {0}", elevation));
                return false;
            }

            var item = ItemsDatabase.Get(itemId);
            var go = GameObjectFactory.StructureItem(item, this.transform);

            if (_items.ContainsKey(elevation))
            {
                _items[elevation] = item;
                _gameObjects[elevation] = go;
            }
            else
            {
                _items.Add(elevation, item);
                _gameObjects.Add(elevation, go);
            }

            return true;
        }

        private Color _currentColor = new Color32(0xFF, 0xFF, 0xFF, 0x00);

        public void OnPointerEnter(PointerEventData eventData)
        {
            var spriteRenderer = this.GetComponent<SpriteRenderer>();
            _currentColor = spriteRenderer.color;
            spriteRenderer.color = new Color32(0x00, 0x00, 0xFF, 0x50);
            //TODO Notify builder
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            var spriteRenderer = this.GetComponent<SpriteRenderer>();
            spriteRenderer.color = _currentColor;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            Log.Info("pew!");
        }
    }
}
