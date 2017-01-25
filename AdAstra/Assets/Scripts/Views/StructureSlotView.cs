using System.Collections.Generic;
using Assets.Scripts.Controllers;
using Assets.Scripts.Db;
using Assets.Scripts.Models;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Scripts.Views
{
    public class StructureSlotView : MonoBehaviour, 
        IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
    {
        private readonly Dictionary<StructureElevation, Item> _items = new Dictionary<StructureElevation, Item>();
        private readonly Dictionary<StructureElevation, GameObject> _gameObjects = new Dictionary<StructureElevation, GameObject>();

        private int _x = -1;
        private int _y = -1;

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
            var go = GameObjectFactory.StructureItem(item, item.StructureBlocking, this.transform);

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

        public bool IsEmpty(StructureElevation structureElevation = StructureElevation.None)
        {
            if(structureElevation == StructureElevation.None) return _gameObjects.Count == 0;
            if (!_gameObjects.ContainsKey(structureElevation)) return true;
            return _gameObjects[structureElevation] == null;
        }
        
        public void OnPointerEnter(PointerEventData eventData)
        {
            //Check if we are in building mode
            if(!BuildController.IsOn()) return;

            var spriteRenderer = this.GetComponent<SpriteRenderer>();
            var isv = ItemSelectionController.GetSelectedSlot();
            var sprite = isv.HasItem
                ? SpritesDatabase.Get(isv.GetItemStack().Item.SpriteName)
                : SpritesDatabase.Get("square");
            spriteRenderer.sprite = sprite;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            //Check if we are in building mode
            if (!BuildController.IsOn()) return;

            var spriteRenderer = this.GetComponent<SpriteRenderer>();
            var sprite = SpritesDatabase.Get("square");
            spriteRenderer.sprite = sprite;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            //Check if we are in building mode
            if (!BuildController.IsOn()) return;

            var canBuild = BuildController.CanBuildInStructureSlot(_x, _y);

            if (canBuild ==  1) Log.Info("CAN BUILD HERE.");
            if (canBuild == -1) Log.Info("CAN NOT BUILD HERE!");
            if (canBuild ==  0) Log.Info("CAN NOT BUILD HERE!");
        }

        public void SetGridPosition(int x, int y)
        {
            _x = x;
            _y = y;
        }
    }
}
