using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Messaging;
using System.Text;
using Assets.Scripts.Db;
using Assets.Scripts.Models;
using Assets.Scripts.Views;
using UnityEngine;

namespace Assets.Scripts.Controllers
{
    public class BuildController: MonoBehaviour
    {
        public static GameObject EscapePod;
        private static EscapePodView EscapePodView;
        private static InventoryController PlayerInventory;

        private static readonly Color AllowedColor = new Color32(0x00, 0xFF, 0x00, 0x40);
        private static readonly Color NotAllowedColor = new Color32(0xFF, 0x00, 0x00, 0x40);
        private static readonly Color BlankColor = new Color32(0xFF, 0xFF, 0xFF, 0x00);
        private static readonly Color NotConstructed = new Color32(0xFF, 0xFF, 0x00, 0x80);

        private static readonly Color BuildableArea = new Color(1f, 1f, 0f, 0.5f);

        public void Start()
        {
            EscapePodView = EscapePod.GetComponent<EscapePodView>();
            PlayerInventory = GameObject.Find("PlayerInventoryController").GetComponent<InventoryController>();
        }

        public void Update()
        {
            BuildModeToggle();
            DebugModeToggle();

            if (Input.GetKeyDown(KeyCode.End))
            {
                EscapePodStructureCheck(null, null, true);
            }

            if (Input.GetKeyDown(KeyCode.Delete))
            {
                EscapePodPressureCheck();
            }
        }

        private static void BuildModeToggle()
        {
            if (!Input.GetKeyDown(KeyCode.Space)) return;

            if (IsOn())
            {
                BuildModeOff();
            }
            else
            {
                BuildModeOn();
            }
        }

        private static void DebugModeToggle()
        {
            if (!Input.GetKeyDown(KeyCode.Backspace)) return;
            DebugShowBuilableArea();
        }


        private static bool _isOn = false;
        public static bool IsOn()
        {
            return _isOn;
        }

        public static void BuildModeOn()
        {
            _isOn = true;
            RefreshBuildOverlay(false);
        }

        public static void BuildModeOff()
        {
            _isOn = false;
            RefreshBuildOverlay(true);
        }

        public static void RefreshBuildOverlay(bool showOnlyNotConstructedObj)
        {
            for (var x = 0; x < EscapePodView.Columns; x++)
            {
                for (var y = 0; y < EscapePodView.Rows; y++)
                {
                    var c = CanBuildInStructureSlot(x, y);
                    switch (c)
                    {
                        case -1:
                            SetBuildOverlayColor(x, y, BlankColor, "square");
                            break;

                        case 0:
                            SetBuildOverlayColor(x, y, showOnlyNotConstructedObj ? BlankColor : NotAllowedColor, "square");
                            break;

                        case 1:
                            SetBuildOverlayColor(x, y, showOnlyNotConstructedObj ? BlankColor : AllowedColor, "square");
                            break;

                        case 2:
                            SetBuildOverlayColor(x, y, NotConstructed, "not_constructed");
                            break;
                    }
                }
            }
        }

        //Return: -1 -> BLANK, 0 - NOT ALLOWED TO BUILD, 1 - ALLOWED, 2 - THERE IS UNCONSTRUCTED OBJ
        public static int CanBuildInStructureSlot(int x, int y)
        {
            var currentItem = ItemSelectionController.GetItem();
            
            //If currently selected item is NOT structure type (or is NULL)
            if (currentItem == null 
                || !currentItem.FunctionProperties.ContainsKey(FunctionProperty.Elevation))
            {
                //If X/Y slot has any unconstructed structures
                if (EscapePodView.GetStructureSlotView(x, y).IsNotConstructed()) return 2;

                //If has not - check if there is adjecent structure
                return HasAdjecentStructure(x, y) ? 0 : -1;
            }

            //If X/Y slot has any unconstructed structures
            if (EscapePodView.GetStructureSlotView(x, y).IsNotConstructed()) return 2;

            //Get selected "structure" item elevation property
            var elevation = currentItem.FunctionProperties.AsEnum<StructureElevation>(FunctionProperty.Elevation);

            //If structure slot is empty...
            if (IsEmpty(x, y))
            {
                //And it's adjecent to existing structure...
                if (HasAdjecentStructure(x, y))
                {
                    //If it's "FIRST" (BelowGroud) layer/elevation 
                    return elevation == StructureElevation.BelowGround ? 1 : 0;
                }

                //If it's not adjecent
                return -1;
            }

            //Structure below is built
            var structureBelowIsBuilt = IsEmpty(x, y, elevation - 1) == false;

            //If item function is MACHINE - it needs to be build on the FLOOR
            if (currentItem.Function == Function.Machine)
            {
                var itemBelow = GetStuctureItemFromElevation(x, y, elevation - 1);
                if (itemBelow == null) return 0;
                return itemBelow.ItemId == ItemId.Floor ? 1 : 0;
            }

            //If structure slot is not empty but SPECIFIC elevation in same slot is:
            return structureBelowIsBuilt && IsEmpty(x, y, elevation) ? 1 : 0;
        }

        private static bool HasAdjecentStructure(int x, int y)
        {
            for (var cX = x - 1; cX <= x + 1; cX++)
            {
                for (var cY = y - 1; cY <= y + 1; cY++)
                {
                    //Ignore center (slot itself) and all diagonal
                    if(cX == x && cY == y) continue;
                    if(cX == x - 1 && cY == y - 1) continue;
                    if(cX == x + 1 && cY == y + 1) continue;
                    if(cX == x - 1 && cY == y + 1) continue;
                    if(cX == x + 1 && cY == y - 1) continue;

                    //Check other ones:
                    var ss = EscapePodView.GetStructureSlotView(cX, cY);
                    if (ss != null && !ss.IsEmpty()) return true;
                }
            }
            return false;
        }

        private static bool IsEmpty(int x, int y, 
            StructureElevation structureElevation = StructureElevation.None)
        {
            return EscapePodView.GetStructureSlotView(x, y).IsEmpty(structureElevation);
        }

        private static Item GetStuctureItemFromElevation(int x, int y, StructureElevation elevation)
        {
            return EscapePodView.GetStructureSlotView(x, y).GetItem(elevation);
        }

        private static void SetBuildOverlayColor(int x, int y, Color color, string sprite)
        {
            var go = EscapePodView.StructureSlots[x,y];
            var marker = go.transform.FindChild("Marker");
            var sr = marker.GetComponent<SpriteRenderer>();
            sr.color = color;
            sr.sprite = SpritesDatabase.Get(sprite);
        }

        public static void Build(int x, int y)
        {
            var currentItem = ItemSelectionController.GetItem();

            if (currentItem == null)
            {
                //This should neve happen!
                Log.Error("BuildController", "Build",
                    string.Format("Strange! Somehow you are " +
                                  "trying to build with NULL item! [X:{0},Y:{1}]!", x, y));
                return;
            }

            //Build!
            var itemId = currentItem.ItemId;
            var success = EscapePodView.AddStructure(x, y, itemId, false);
            if (success)
            {
                ItemSelectionController.GetItemStackView().UpdateStackCount(-1);
                RefreshBuildOverlay(false);
            }
            else
            {
                //This should neve happen!
                Log.Error("BuildController", "Build",
                    string.Format("Strange! Somehow you are were unable to " +
                                  "build in here! [X:{0},Y:{1}]!", x, y));
            }
        }
        
        public static bool CanRemove(int x, int y, Item item, bool ignoreStructureCheck)
        {
            var spaceInInventory = PlayerInventory
                .CheckAddItem(new Dictionary<ItemId, int>() { { item.ItemId, 1 } }).Count == 0;

            //Check if we have space in the inventory...
            if (spaceInInventory == false)
            {
                //Player doesn't have enough space in the inventory
                NotificationFeedController.Add(Icons.Error, "Not enough space in the inventory.");
                return false;
            }

            //If we are ignoring structure check we can bail out here
            if (ignoreStructureCheck) return true;

            //... and removing this structure will not "fracture" Escape Pod
            if (EscapePodStructureCheck(x, y, false) == false)
            {
                //Player doesn't have enough space in the inventory
                NotificationFeedController.Add(Icons.Error, "You can not remove this object.");
                return false;
            }

            //Otherwise we are ok to remove it
            return true;
        }

        public static void Remove(int x, int y, StructureElevation elevation, Item item)
        {
            //Check if we can return this item to player's inventory
            //We can ignore structure check if we are removing obj above "BelowGround"
            //(as there still will be foundation below)
            if (!CanRemove(x, y, item, elevation > StructureElevation.BelowGround)) return;
            EscapePodView.RemoveStructure(x, y, elevation);
            PlayerInventory.AddItem(item.ItemId, 1);
            RefreshBuildOverlay(false);
        }

        private static void DebugShowBuilableArea()
        {
            for (var x = 0; x < EscapePodView.Columns; x++)
            {
                for (var y = 0; y < EscapePodView.Rows; y++)
                {
                    SetBuildOverlayColor(x, y, BuildableArea, "square");
                }
            }
        }

        #region INTEGRITY CHECK
        private static SimpleTile[,] _airMap;
        private enum SimpleTile
        {
            Empty,
            Floor, 
            Wall,
            FloodedFloor,
            PressurizedFloor,
            NotPressurizedFloor
        }

        //Check which areas are fully enclosed and pressurized
        private static void EscapePodPressureCheck()
        {
            //Create simplified map
            _airMap = new SimpleTile[EscapePodView.Columns, EscapePodView.Rows];
            var firstFloorX = -1;
            var firstFloorY = -1;

            for (var x = 0; x < EscapePodView.Columns; x++)
            {
                for (var y = 0; y < EscapePodView.Rows; y++)
                {
                    var ssv = EscapePodView.GetStructureSlotView(x, y);
                    var groundItem = ssv.GetItem(StructureElevation.Ground);
                    if (groundItem == null)
                    {
                        _airMap[x, y] = SimpleTile.Empty;
                    }
                    else
                    {
                        if (groundItem.ItemId == ItemId.Floor)
                        {
                            _airMap[x, y] = SimpleTile.Floor;

                            //Set 1st floor
                            if (firstFloorX != -1 || firstFloorY != -1) continue;
                            firstFloorX = x;
                            firstFloorY = y;
                        }
                        else if (groundItem.ItemId == ItemId.Wall)
                        {
                            _airMap[x, y] = SimpleTile.Wall;
                        }
                        else
                        {
                            _airMap[x, y] = SimpleTile.Empty;
                        }
                    }
                }
            }

            //Floor flood
            FloorFlood(firstFloorX, firstFloorY);
            
            //Debug output
            for (var x = 0; x < EscapePodView.Columns; x++)
            {
                for (var y = 0; y < EscapePodView.Rows; y++)
                {
                    var v = _airMap[x, y];
                    var color = Color.white;
                    switch (v)
                    {
                        case SimpleTile.Empty:
                            color = Color.clear;
                            break;

                        case SimpleTile.Floor:
                            color = Color.yellow;
                            break;

                        case SimpleTile.Wall:
                            color = Color.gray;
                            break;

                        case SimpleTile.FloodedFloor:
                            color = Color.magenta;
                            break;

                        case SimpleTile.PressurizedFloor:
                            color = Color.green;
                            break;

                        case SimpleTile.NotPressurizedFloor:
                            color = Color.red;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    SetBuildOverlayColor(x, y, color, "square");
                }
            }
        }

        private static void FloorFlood(int x, int y)
        {
            if (x < 0 || x >= EscapePodView.Columns) return;
            if (y < 0 || y >= EscapePodView.Rows) return;

            if (_airMap[x, y] == SimpleTile.Floor)
            {
                _airMap[x, y] = NeighborsCheck(x,y) ? SimpleTile.PressurizedFloor : SimpleTile.NotPressurizedFloor;
                FloorFlood(x, y);
                FloorFlood(x + 1, y);
                FloorFlood(x, y + 1);
                FloorFlood(x - 1, y);
                FloorFlood(x, y - 1);
            }
        }

        private static bool NeighborsCheck(int x, int y)
        {
            for (var nX = x - 1; nX <= x + 1; nX++)
            {
                for (var nY = y - 1; nY <= y + 1; nY++)
                {
                    if (nX == x && nY == y) continue;
                    if (nX < 0 || nX > EscapePodView.Columns - 1 || nY < 0 || nY > EscapePodView.Rows - 1) continue;

                    if (_airMap[nX, nY] != SimpleTile.Floor
                        && _airMap[nX, nY] != SimpleTile.Wall
                        && _airMap[nX, nY] != SimpleTile.PressurizedFloor)
                    {
                        return false;
                    }
                }
            }

            return true;
        }
        


        //Check if EscapePod is as ONE Structure and it's not "in pieces"
        //Pass through x/y of possible new "gap" 
        private static int[,] _map;

        private static bool EscapePodStructureCheck(int? virtualX, int? virtualY, bool debugMode)
        {
            //Create simplified map
            _map = new int[EscapePodView.Columns, EscapePodView.Rows];
            var firstNotEmptyX = -1;
            var firstNotEmptyY = -1;

            for (var x = 0; x < EscapePodView.Columns; x++)
            {
                for (var y = 0; y < EscapePodView.Rows; y++)
                {
                    var isEmpty = EscapePodView.GetStructureSlotView(x, y).IsEmpty(StructureElevation.BelowGround);
                    _map[x, y] = isEmpty ? 0 : 1;

                    //Set 1st non empty position
                    if (!isEmpty && firstNotEmptyX == -1 && firstNotEmptyY == -1)
                    {
                        firstNotEmptyX = x;
                        firstNotEmptyY = y;
                    }
                }
            }

            //Add the one tile we are testing
            if (virtualX.HasValue && virtualY.HasValue)
            {
                _map[virtualX.Value, virtualY.Value] = 0;
            }

            //Flood - flood with [-1] and go through only tiles with value [1]
            Flood(firstNotEmptyX, firstNotEmptyY, -1, 1);

            //Are there any non-flooded non-empty tiles left?
            var allCorrect = true;
            for (var x = 0; x < EscapePodView.Columns; x++)
            {
                for (var y = 0; y < EscapePodView.Rows; y++)
                {
                    var v = _map[x, y];

                    //In debug mode we will go through all tiles 
                    if (debugMode)
                    {
                        var color = Color.white;
                        if (v == -1) color = Color.magenta; //Flooded
                        if (v == 0) color = Color.white; //Empty
                        if (v == 1) color = Color.blue; //Incorrect!
                        SetBuildOverlayColor(x, y, color, "square");
                        if (v == 1) allCorrect = false;
                    }
                    //In normal mode we will return if we found first incorrect tile
                    else
                    {
                        if (v == 1) return false;
                    }
                }
            }

            Log.Info("EscapePod status: " + (allCorrect ? "All OK" : "FRACTURED!!!"));
            return allCorrect;
        }

        private static void Flood(int x, int y, int fillWith, int searchFor)
        {
            if (x < 0 || x >= EscapePodView.Columns) return;
            if (y < 0 || y >= EscapePodView.Rows) return;

            if (_map[x, y] == searchFor)
            {
                _map[x, y] = fillWith;
                Flood(x + 1, y, fillWith, searchFor);
                Flood(x, y + 1, fillWith, searchFor);
                Flood(x - 1, y, fillWith, searchFor);
                Flood(x, y - 1, fillWith, searchFor);
            }
        }

        #endregion
    }
}
