using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Assets.Scripts.Controllers;
using Assets.Scripts.Db;
using Assets.Scripts.Models;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Scripts.Views
{
    public class StructureSlotView : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler,
        IPointerClickHandler, IPointerDownHandler, IPointerUpHandler 
    {
        private readonly Dictionary<StructureElevation, Item> _items = new Dictionary<StructureElevation, Item>();
        private readonly Dictionary<StructureElevation, GameObject> _gameObjects = new Dictionary<StructureElevation, GameObject>();
        private readonly Dictionary<StructureElevation, float> _construction = new Dictionary<StructureElevation, float>();

        private int _x = -1;
        private int _y = -1;

        //Returns TRUE is succesfully added, false if it already has a structure on this elevation
        public bool AddStructure(StructureElevation elevation, ItemId itemId, bool instaBuild)
        {
            //Item (structure) already exist
            if (_items.ContainsKey(elevation) && _items[elevation] != null)
            {
                Log.Warn("StructureSlotView", "AddStructure", 
                    string.Format("Structure already exist on Elevation = {0}", elevation));
                return false;
            }

            var item = ItemsDatabase.Get(itemId);
            var go = GameObjectFactory.StructureItem(item, item.StructureBlocking, elevation, this.transform);
            
            if (_items.ContainsKey(elevation))
            {
                _items[elevation] = item;
                _gameObjects[elevation] = go;
                _construction[elevation] = instaBuild ? 0.0f : item.ConstructionTime;
            }
            else
            {
                _items.Add(elevation, item);
                _gameObjects.Add(elevation, go);
                _construction.Add(elevation, instaBuild ? 0.0f : item.ConstructionTime);
            }

            return true;
        }

        public void SetGridPosition(int x, int y)
        {
            _x = x;
            _y = y;
        }

        public bool IsEmpty(StructureElevation structureElevation = StructureElevation.None)
        {
            if(structureElevation == StructureElevation.None) return _gameObjects.Count == 0;
            if (!_gameObjects.ContainsKey(structureElevation)) return true;
            return _gameObjects[structureElevation] == null;
        }

        public bool IsNotConstructed()
        {
            if (_construction.Count == 0) return false;
            foreach (var c in _construction)
            {
                if (c.Value > 0.0f)
                {
                    return true;
                }
            }
            return false;
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

        public void OnPointerClick(PointerEventData eventData)
        {
            //Check if we are in building mode
            if (!BuildController.IsOn()) return;

            var canBuild = BuildController.CanBuildInStructureSlot(_x, _y);

            switch (canBuild)
            {
                //To far to build
                case -1:
                    Log.Info("StructureSlotView", "OnPointerDown",
                        string.Format("Can NOT build here: [X:{0},Y:{1}]!", _x, _y));
                    break;

                //Can build her!
                case 1:
                    BuildController.Build(_x, _y);
                    break;

                //Can NOT build here
                case 0:
                    Log.Info("StructureSlotView", "OnPointerDown",
                        string.Format("Can NOT build here: [X:{0},Y:{1}]!", _x, _y));
                    break;
            }
        }

        #region CONSTRUCTION
        private bool _constructing = false;

        public void OnPointerDown(PointerEventData eventData)
        {
            if (!BuildController.IsOn() && IsNotConstructed())
            {
                _constructing = true;
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (_constructing)
            {
                _constructing = false;
            }
        }

        public void Update()
        {
            if(BuildController.IsOn()) return;
            if (!_constructing) return;
            //TODO Chenge selection - select first elevation from donw to up 
            //TODO with construction time
            var structureElevation = _construction.Keys.First();
            _construction[structureElevation] -= Time.deltaTime;
            if (_construction[structureElevation] <= 0.0f)
            {
                _construction[structureElevation] = 0.0f;
                _constructing = false;
                BuildController.RefreshNotContructedStructuresOverlay();
            }
            Log.Info(_construction[structureElevation].ToString(CultureInfo.InvariantCulture));
        }
        #endregion

    }
}
