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
            var isStructure = item.Function == Function.Structure;
            var cTime = item.FunctionProperties.AsFloat(isStructure ? FunctionProperty.ConstructionTime : FunctionProperty.AssemblyTime);

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

            //Check if selected item is of a structure type...
            //TODO Include other placable items like machines which are not structures
            var item = ItemSelectionController.GetItem();
            var marker = this.transform.FindChild("Marker");
            var empty = SpritesDatabase.Get("square");

            //...If not keep empty overlay 
            if (item == null || !item.FunctionProperties.ContainsKey(FunctionProperty.Elevation))
            {
                marker.GetComponent<SpriteRenderer>().sprite = empty;
                return;
            }

            //otherwise change "overlay" sprite to currently selected "structure" item
            marker.GetComponent<SpriteRenderer>().sprite = SpritesDatabase.Get(item.SpriteName);
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
            var selectedItem = ItemSelectionController.GetItemStackView();
            if (selectedItem.HasItem)
            {
                var item = selectedItem.GetItemStack().Item;
                var isTool = item.Function == Function.Tool;
                if (!isTool)
                {
                    Log.Info("StructureSlotView", "OnPointerDown", "Construction tool not equipped!");
                    return;
                }

                //If constructing object is a STRUCTURE, player needs to have equiped "Constructor"
                if (GetNotConstructedObjectFunction() == Function.Structure && item.ItemId != ItemId.Constructor)
                {
                    Log.Info("StructureSlotView", "OnPointerDown", "Constructor not equipped!");
                    return;
                }

                //If constructing object is a MACHINE, player needs to have equiped "Assembler"
                if (GetNotConstructedObjectFunction() == Function.Machine && item.ItemId != ItemId.Assembler)
                {
                    Log.Info("StructureSlotView", "OnPointerDown", "Assembler tool not equipped!");
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
            var elevation = GetNotConstructedObjectElevation();

            //Nothing to construct
            if (elevation == StructureElevation.None)
            {
                Log.Error("StructureSlotView", "Update", 
                    string.Format("Trying to construct but nothing to construct! [{0},{1}]", _x, _y));
                return;
            }

            //Establish if we are constructing or assembling
            //True - STRUCTURE # False - MACHINE
            var isStructure = GetNotConstructedObjectFunction() == Function.Structure;
            
            //Get construction/assembly speed (depend if we are working on structure or machine)
            //If we got so far here - we definitelly have equipped correct tool
            var selectedItem = ItemSelectionController.GetItemStackView().GetItemStack().Item;
            var workSpeed = isStructure 
                ? selectedItem.FunctionProperties.AsFloat(FunctionProperty.ConstructionSpeed)
                : selectedItem.FunctionProperties.AsFloat(FunctionProperty.AssemblySpeed);

            //Construct!
            var maxTime = isStructure
                ? _items[elevation].FunctionProperties.AsFloat(FunctionProperty.ConstructionTime)
                : _items[elevation].FunctionProperties.AsFloat(FunctionProperty.AssemblyTime);

            _construction[elevation] -= (Time.deltaTime * workSpeed);
            WorkProgressController.UpdateWork(_construction[elevation], maxTime);

            //Construction finished!
            if (_construction[elevation] <= 0.0f)
            {
                WorkProgressController.Off();
                _construction[elevation] = 0.0f;
                var go = _gameObjects[elevation];
                go.GetComponent<SpriteRenderer>().color = Color.white;
                var item = _items[elevation];
                var isBlocking = item.FunctionProperties.AsBool(FunctionProperty.IsBlocking);
                go.GetComponent<BoxCollider2D>().enabled = isBlocking;

                _constructing = false;
                BuildController.RefreshNotContructedStructuresOverlay();
            }

            //Log.Info(_construction[structureElevation].ToString(CultureInfo.InvariantCulture));
        }

        private StructureElevation GetNotConstructedObjectElevation()
        {
            var elevation = StructureElevation.None;
            foreach (var c in _construction)
            {
                if (!(c.Value > 0.0f)) continue;
                elevation = c.Key;
                break;
            }
            return elevation;
        }

        private Function GetNotConstructedObjectFunction()
        {
            var elevation = GetNotConstructedObjectElevation();
            return elevation == StructureElevation.None ? Function.None : _items[elevation].Function;
        }

        #endregion

        #region GET
        public Item GetItem(StructureElevation elevation)
        {
            return _items.ContainsKey(elevation) ? _items[elevation] : null;
        }

        public GameObject GetGo(StructureElevation elevation)
        {
            return _gameObjects.ContainsKey(elevation) ? _gameObjects[elevation] : null;
        }
        #endregion

    }
}
