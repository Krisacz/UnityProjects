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

        #region ADD STRUCTURE
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
            var isBlocking = item.FunctionProperties.AsBool(FunctionProperty.IsBlocking);
            var go = GameObjectFactory.StructureItem(item, isBlocking, elevation, this.transform);
            var cTime = item.FunctionProperties.AsFloat(FunctionProperty.ConstructionTime);

            if (!instaBuild)
            {
                //TODO Consider this: make all sturtures below this one invisible to give
                //TODO player better feedback what is in this slot and whats need to be build
                //TODO Or some other representation of "not cnostructed" item
                go.GetComponent<SpriteRenderer>().color = new Color32(0xFF, 0xFF, 0xFF, 0x80);
                go.GetComponent<BoxCollider2D>().enabled = !isBlocking;
            }

            if (_items.ContainsKey(elevation))
            {
                _items[elevation] = item;
                _gameObjects[elevation] = go;
                _construction[elevation] = instaBuild ? 0.0f : cTime;
            }
            else
            {
                _items.Add(elevation, item);
                _gameObjects.Add(elevation, go);
                _construction.Add(elevation, instaBuild ? 0.0f : cTime);
            }

            return true;
        }
        #endregion

        #region SET GRID POSITION
        public void SetGridPosition(int x, int y)
        {
            _x = x;
            _y = y;
        }
        #endregion
        
        #region IS EMPTY
        public bool IsEmpty(StructureElevation structureElevation = StructureElevation.None)
        {
            if (structureElevation == StructureElevation.None) return _gameObjects.Count == 0;
            if (!_gameObjects.ContainsKey(structureElevation)) return true;
            return _gameObjects[structureElevation] == null;
        }
        #endregion

        #region IS NOT CONSTRUCTED
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
        #endregion
        
        #region ON POINTER ENTER
        public void OnPointerEnter(PointerEventData eventData)
        {
            //Check if we are in building mode
            if (!BuildController.IsOn()) return;

            //Change "overlay" sprite to current item
            var isv = ItemSelectionController.GetSelectedSlot();
            var sprite = isv.HasItem
                ? SpritesDatabase.Get(isv.GetItemStack().Item.SpriteName)
                : SpritesDatabase.Get("square");

            var marker = this.transform.FindChild("Marker");
            marker.GetComponent<SpriteRenderer>().sprite = sprite;
        }
        #endregion

        #region ON POINTER EXIT
        public void OnPointerExit(PointerEventData eventData)
        {
            //Check if we are in building mode
            if (!BuildController.IsOn()) return;

            var sprite = SpritesDatabase.Get("square");
            var marker = this.transform.FindChild("Marker");
            marker.GetComponent<SpriteRenderer>().sprite = sprite;
        }
        #endregion

        #region ON POINTER CLICK - BUILDING NEW STRUCTURE
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
        #endregion

        #region CONSTRUCTION
        private bool _constructing = false;

        public void OnPointerDown(PointerEventData eventData)
        {
            //Only in NON Build mode
            if(BuildController.IsOn()) return;

            //If all are already constructed - bail out
            if(!IsNotConstructed()) return;

            //Check if distance is ok
            var p = PlayerController.GetBounds().center;
            var s = GetComponent<BoxCollider2D>().bounds.center;
            var distance = Vector2.Distance(p, s);
            if (distance > 2.0f)
            {
                Log.Info("StructureSlotView", "OnPointerDown", "Player is too far to build!");
                return;
            }

            //Check if player is not standing on building structure
            if (PlayerController.GetBounds().Intersects(GetComponent<BoxCollider2D>().bounds))
            {
                Log.Info("StructureSlotView", "OnPointerDown", "Player is too close to build!");
                return;    
            }

            //Check if player is holding appropriate tool
            var selectedItem = ItemSelectionController.GetSelectedSlot();
            if (selectedItem.HasItem)
            {
                var item = selectedItem.GetItemStack().Item;
                var isTool = item.Function == Function.Tool;
                if (!isTool)
                {
                    Log.Info("StructureSlotView", "OnPointerDown", "Construction tool not equipped!");
                    return;
                }

                var canConstruct = item.FunctionProperties.ContainsKey(FunctionProperty.ConstructionSpeed);
                if (!canConstruct)
                {
                    Log.Info("StructureSlotView", "OnPointerDown", "Construction tool not equipped!");
                    return;
                }
            }
            else
            {
                Log.Info("StructureSlotView", "OnPointerDown", "Construction tool not equipped!");
                return;
            }
             
            _constructing = true;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (_constructing)
            {
                _constructing = false;
                WorkProgressController.Off();
            }
        }

        public void Update()
        {
            //In a build mode or not constructing
            if(BuildController.IsOn()) return;
            if (!_constructing) return;

            //Get first not constructed structure
            var structureElevation = StructureElevation.None;
            foreach (var c in _construction)
            {
                if (c.Value > 0.0f)
                {
                    structureElevation = c.Key;
                    break;
                }
            }

            //Nothing to construct
            if (structureElevation == StructureElevation.None)
            {
                Log.Error("StructureSlotView", "Update", 
                    string.Format("Trying to construct but nothing to construct! [{0},{1}]", _x, _y));
                return;
            }

            //Get construction speed
            var selectedItem = ItemSelectionController.GetSelectedSlot().GetItemStack().Item;
            var constructionSpeed = selectedItem.FunctionProperties
                .AsFloat(FunctionProperty.ConstructionSpeed);
                
            //Construct!
            _construction[structureElevation] -= (Time.deltaTime * constructionSpeed);
            WorkProgressController.UpdateWork(_construction[structureElevation], 
                _items[structureElevation].FunctionProperties.AsFloat(FunctionProperty.ConstructionTime));

            //Construction finished!
            if (_construction[structureElevation] <= 0.0f)
            {
                WorkProgressController.Off();
                _construction[structureElevation] = 0.0f;
                var go = _gameObjects[structureElevation];
                go.GetComponent<SpriteRenderer>().color = Color.white;
                var item = _items[structureElevation];
                var isBlocking = item.FunctionProperties.AsBool(FunctionProperty.IsBlocking);
                go.GetComponent<BoxCollider2D>().enabled = isBlocking;

                _constructing = false;
                BuildController.RefreshNotContructedStructuresOverlay();
            }

            //Log.Info(_construction[structureElevation].ToString(CultureInfo.InvariantCulture));
        }
        #endregion

    }
}
