using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Assets.Scripts.Controllers;
using Assets.Scripts.Db;
using Assets.Scripts.Models;
using JetBrains.Annotations;
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

        private WorkState _workState = WorkState.None;

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
                go.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0.25f);
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

        #region REMOVE STRUCTURE
        public void RemoveStructure(StructureElevation elevation)
        {
            try
            {
                _construction.Remove(elevation);
                _items.Remove(elevation);
                var go = _gameObjects[elevation];
                _gameObjects.Remove(elevation);
                Destroy(go);
            }
            catch (Exception ex)
            {
                Log.Error("StructureSlotView", "RemoveStructure", 
                    string.Format("Error while removing structure from [{0},{1}] on elevation: {2}\nError: {3}",
                    _x, _y, elevation, ex));
            }
        }
        #endregion

        #region SET GRID POSITION
        public void SetGridPosition(int x, int y)
        {
            _x = x;
            _y = y;
        }
        #endregion
        
        #region ON POINTER ENTER
        public void OnPointerEnter(PointerEventData eventData)
        {
            //Check if we are in building mode
            if (!BuildController.IsOn()) return;

            //Get some data...
            var item = ItemSelectionController.GetItem();
            var marker = this.transform.FindChild("Marker");
            var empty = SpritesDatabase.Get("square");
            var notConstructed = SpritesDatabase.Get("not_constructed");
            var isNotConstructed = IsNotConstructed();

            //Check if item is of "buildable" type
            if (item == null || !item.FunctionProperties.ContainsKey(FunctionProperty.Elevation))
            {
                marker.GetComponent<SpriteRenderer>().sprite = isNotConstructed ? notConstructed : empty;
                return;
            }

            //otherwise change "overlay" sprite to currently selected "structure" item
            //unless it's not constructed then "keep" not constructed overlay
            var currentItem = SpritesDatabase.Get(item.SpriteName);
            marker.GetComponent<SpriteRenderer>().sprite = isNotConstructed ? notConstructed : currentItem;
        }
        #endregion

        #region ON POINTER EXIT
        public void OnPointerExit(PointerEventData eventData)
        {
            //Check if we are in building mode
            if (!BuildController.IsOn())
            {
                if(_workState == WorkState.None) return;

                //Stop the process / visuals
                _workState = WorkState.None;
                WorkProgressController.Off();
                PlayerController.GetLineController().HideLine();
                return;
            }

            var sprite = SpritesDatabase.Get(IsNotConstructed() ? "not_constructed" : "square");
            var marker = this.transform.FindChild("Marker");
            marker.GetComponent<SpriteRenderer>().sprite = sprite;
        }
        #endregion

        #region ON POINTER CLICK - BUILDING NEW STRUCTURE
        public void OnPointerClick(PointerEventData eventData)
        {
            //Check if we are in building mode
            if (!BuildController.IsOn()) return;

            //Check if we already have "not constructed" object here
            //And if we are clicking on it again - we are cancelling build 
            //and rteturning item to our inventory if we have a space
            if (IsNotConstructed())
            {
                var elevation = GetNotConstructedObjectElevation();
                var item = _items[elevation];
                BuildController.Remove(_x, _y, elevation, item);
                return;
            }
            
            //Check if we can build here
            var canBuild = BuildController.CanBuildInStructureSlot(_x, _y);

            switch (canBuild)
            {
                //To far to build
                case -1:
                    Log.Info("StructureSlotView", "OnPointerDown",
                        string.Format("Can NOT build here: [X:{0},Y:{1}]!", _x, _y));
                    break;

                //Can build here!
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

        public void OnPointerDown(PointerEventData eventData)
        {
            //Only in NON Build mode
            if(BuildController.IsOn()) return;

            //If structure slot is empty there is nothing we can do here
            if(IsEmpty()) return;

            //Holding Tool check
            if (!CheckEquippedItem()) return;

            //Check for distance, overlapping and line of sight to the constructing/assembling object
            if (!CheckDistanceCollisionLoS()) return;

            //If there are objs here and all of them are constructed 
            //then we are trying to "deconstruct" this object
            if (!IsEmpty() && !IsNotConstructed())
            {
                //Check if we can return this obj to player inventory after it will be deconstructed
                var objElevation = GetTopMostConstructedObjectElevation();
                if(!BuildController.CanRemove(_x, _y, _items[objElevation], objElevation > StructureElevation.BelowGround))
                {
                    _workState = WorkState.None;
                    return;
                }

                //Set work type
                var objFunction = GetTopMostConstructedObjectFunction();
                switch (objFunction)
                {
                    case Function.None:
                    case Function.Resource:
                    case Function.Tool:
                        Log.Error("StructureSlotView", "OnPointerDown", "Not implemented!");
                        break;

                    case Function.Structure:
                        _workState = WorkState.Deconstructing;
                        break;

                    case Function.Machine:
                        _workState = WorkState.Disassembling;
                        break;

                    default:    throw new ArgumentOutOfRangeException();
                }
            }
            //otherwise we are trying to contruct/assemble obj
            else
            {
                var objFunction = GetNotConstructedObjectFunction();
                switch (objFunction)
                {
                    case Function.None:
                    case Function.Resource:
                    case Function.Tool:
                        Log.Error("StructureSlotView", "OnPointerDown", "Not implemented!");
                        break;

                    case Function.Structure:
                        _workState = WorkState.Constructing;
                        break;

                    case Function.Machine:
                        _workState = WorkState.Assembling;
                        break;

                    default:    throw new ArgumentOutOfRangeException();
                }
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (_workState == WorkState.None) return;

            //Stop the process / visuals
            WorkProgressController.Off();
            PlayerController.GetLineController().HideLine();

            switch (_workState)
            {
                case WorkState.Constructing:
                case WorkState.Assembling:
                    _workState = WorkState.None;
                    return;

                //Whole process needs to be done in one go - 
                //not like building where you can build partially
                //So as a result we are reseting work counter
                case WorkState.Deconstructing:
                case WorkState.Disassembling:
                    ResetUncompletedDestructionObj();
                    _workState = WorkState.None;
                    return;

                default:    throw new ArgumentOutOfRangeException();
            }
        }
        
        #region VALIDITY CHECKS
        private bool CheckEquippedItem()
        {
            //Check if player is holding appropriate tool
            var selectedItem = ItemSelectionController.GetItemStackView();

            if (!selectedItem.HasItem)
            {
                NotificationFeedController.Add(Icons.Error, "Appropriate tool needs to be equipped");
                Log.Info("StructureSlotView", "OnPointerDown", "Appropriate tool needs to be equipped!");
                return false;
            }

            var item = selectedItem.GetItemStack().Item;
            var isTool = item.Function == Function.Tool;
            if (!isTool)
            {
                Log.Info("StructureSlotView", "OnPointerDown", "Appropriate tool needs to be equipped!");
                return false;
            }

            //If constructing object is a STRUCTURE, player needs to have equiped "Constructor"
            var objFunc = GetNotConstructedObjectFunction();
            if (objFunc == Function.Structure && item.ItemId != ItemId.Constructor)
            {
                Log.Info("StructureSlotView", "OnPointerDown", "Constructor not equipped!");
                return false;
            }

            //If constructing object is a MACHINE, player needs to have equiped "Assembler"
            if (objFunc == Function.Machine && item.ItemId != ItemId.Assembler)
            {
                Log.Info("StructureSlotView", "OnPointerDown", "Assembler tool not equipped!");
                return false;
            }

            return true;
        }

        private bool CheckDistanceCollisionLoS()
        {
            //Check if distance is ok
            var p = PlayerController.GetBounds().center;
            var s = this.GetComponent<BoxCollider2D>().bounds.center;
            var distance = Vector2.Distance(p, s);

            if (distance > 2.0f)
            {
                Log.Info("StructureSlotView", "OnPointerDown", "Player is too far to build!");
                return false;
            }

            //Check if player is not standing on building structure
            if (PlayerController.GetBounds().Intersects(GetComponent<BoxCollider2D>().bounds))
            {
                Log.Info("StructureSlotView", "OnPointerDown", "Player is too close to build!");
                return false;
            }

            //Check if player "can see" contructing/assembling object 
            var direction = (s - p).normalized;
            var hits = Physics2D.RaycastAll(p, direction, distance);

            //Is object on blockin layer and NOT same as we are targeting? (if we are deconstructing e.g.) 
            var beingBlocked = hits.Any(x => x.transform.gameObject.layer == (int) Layer.ModuleBlocking
            && !x.transform.GetComponent<BoxCollider2D>().bounds.center.Equals(s));

            if (beingBlocked)
            {
                Log.Info("StructureSlotView", "OnPointerDown", "Player can not see the Target!");
                return false;
            }

            return true;
        }
        #endregion

        #region UPDATE
        public void Update()
        {
            //Not in a build mode / nothing to work on
            if (BuildController.IsOn()) return;
            if (_workState == WorkState.None) return;

            //Check if we are creating or destroying
            var state = default(bool?);
            if (_workState == WorkState.Constructing || _workState == WorkState.Assembling)
            {
                state = true;
            }
            else if (_workState == WorkState.Deconstructing || _workState == WorkState.Disassembling)
            {
                state = false;
            }
            //Did we missed some new WorkState or was WorkState = None?
            if (!state.HasValue) throw new Exception("StructureSlotView >> Update >> You missed something here!");
            
            //Check if "contructing process" is stil valid
            var checkEquippedItem = CheckEquippedItem();
            var checkDistanceCollisionLoS = CheckDistanceCollisionLoS();
            if (!checkEquippedItem || !checkDistanceCollisionLoS)
            {
                //Reset uncompleted destruction of the obj
                if (state.Value == false) ResetUncompletedDestructionObj();

                //Stop the process / visuals
                _workState = WorkState.None;
                WorkProgressController.Off();
                PlayerController.GetLineController().HideLine();
                
                Log.Warn("StructureSlotView", "Update", string.Format("Stopped contruction/assembling! Reason: {0}|{1}",
                    checkEquippedItem == false ? "Equipped Item" : string.Empty, 
                    checkDistanceCollisionLoS == false ? "Distance Collision LOS" : string.Empty));
                return;
            }

            //If we are creating get first not constructed structure elevation
            //otherwise get top-most fully-constructed obj elevation
            var elevation = state.Value ? GetNotConstructedObjectElevation() : GetTopMostConstructedObjectElevation();

            //Nothing to construct
            if (elevation == StructureElevation.None)
            {
                Log.Error("StructureSlotView", "Update", string.Format("Trying to construct but nothing to construct! [{0},{1}]", _x, _y));
                return;
            }

            //Get selected item
            var selectedItem = ItemSelectionController.GetItemStackView().GetItemStack().Item;
            var workSpeed = float.MaxValue;
            var maxTime = float.MaxValue;

            //Check the work type
            if (_workState == WorkState.Constructing || _workState == WorkState.Deconstructing)
            {
                //Get construction speed
                workSpeed = selectedItem.FunctionProperties.AsFloat(FunctionProperty.ConstructionSpeed);
                maxTime = _items[elevation].FunctionProperties.AsFloat(FunctionProperty.ConstructionTime);
            }
            else if (_workState == WorkState.Assembling || _workState == WorkState.Disassembling)
            {
                //Get assembling speed
                workSpeed = selectedItem.FunctionProperties.AsFloat(FunctionProperty.AssemblySpeed);
                maxTime = _items[elevation].FunctionProperties.AsFloat(FunctionProperty.AssemblyTime);
            }
            else
            {
                Log.Error("StructureSlotView", "Update", "Not Implemented!");
            }
            
            //====== Do Work!
            _construction[elevation] -= (Time.deltaTime*workSpeed);
            var currentValue = Math.Abs(_construction[elevation]);
            var color = state.Value ? Color.blue : Color.red;
            WorkProgressController.UpdateWork(currentValue, maxTime, color);
            PlayerController.GetLineController().ShowLine(_gameObjects[elevation].transform.position, color);
            var newAlpha = GetAlphaFromProgress(currentValue, maxTime, state.Value);
            var go = _gameObjects[elevation];
            go.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, newAlpha);

            //Work finished! - if we were contructing then work time needs to be 0 or just below
            //If we were decontructing work time needs to be below zero and below maxwork time - negative value!
            if (state.Value ? _construction[elevation] <= 0.0f : _construction[elevation] <= -1*maxTime)
            {
                //Stop the process / visuals
                _workState = WorkState.None;
                WorkProgressController.Off();
                PlayerController.GetLineController().HideLine();

                //Get item
                var item = _items[elevation];

                //Construcing/Assembling
                if (state.Value)
                {
                    //Set object to fully constructed/assembled
                    _construction[elevation] = 0.0f;
                    go.GetComponent<SpriteRenderer>().color = Color.white;
                    var isBlocking = item.FunctionProperties.AsBool(FunctionProperty.IsBlocking);
                    go.GetComponent<BoxCollider2D>().enabled = isBlocking;
                }
                //Deconstructing / Disassembling
                else
                {
                    //Remove obj
                    BuildController.Remove(_x, _y, elevation, item);
                }

                //Refresh visuals
                BuildController.RefreshBuildOverlay(true);
            }
        }

        private static float GetAlphaFromProgress(float current, float max, bool appear)
        {
            //Constuction / Assembly
            if (appear)
            {
                // When placing object it's initial alpha is 0.25f
                return Mathf.Clamp(1f - (current / max) * 0.75f, 0f, 1f);
            }

            //Deconstruction / Disassembly
            return Mathf.Clamp(1f - (current / max), 0f, 1f);
        }

        #endregion

        #region OBJECT INFO / HELP METHODS
        public Item GetItem(StructureElevation elevation)
        {
            return _items.ContainsKey(elevation) ? _items[elevation] : null;
        }

        public bool IsEmpty(StructureElevation structureElevation = StructureElevation.None)
        {
            if (structureElevation == StructureElevation.None) return _gameObjects.Count == 0;
            if (!_gameObjects.ContainsKey(structureElevation)) return true;
            return _gameObjects[structureElevation] == null;
        }

        public bool IsNotConstructed()
        {
            if (_construction.Count == 0) return false;
            return _construction.Any(x => x.Value > 0f);
        }

        private void ResetUncompletedDestructionObj()
        {
            var constructionElevation = GetTopMostConstructedObjectElevation();
            _construction[constructionElevation] = 0f;
            var cGo = _gameObjects[constructionElevation];
            cGo.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1f);
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

        private StructureElevation GetTopMostConstructedObjectElevation()
        {
            return _construction.Where(w => w.Value <= 0f).Max(e => e.Key);
        }

        private Function GetTopMostConstructedObjectFunction()
        {
            var elevation = GetTopMostConstructedObjectElevation();
            return elevation == StructureElevation.None ? Function.None : _items[elevation].Function;
        }
        #endregion
    }

    internal enum WorkState
    {
        None,

        Constructing,
        Deconstructing,

        Assembling,
        Disassembling
    }
}
