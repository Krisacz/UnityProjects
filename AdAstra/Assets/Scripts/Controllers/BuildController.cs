using System.Collections.Generic;
using Assets.Scripts.Db;
using Assets.Scripts.Models;
using Assets.Scripts.Views;
using UnityEngine;

namespace Assets.Scripts.Controllers
{
    public class BuildController: MonoBehaviour
    {
        public static GameObject EscapePod;
        public static InventoryController InventoryController;

        private static EscapePodView _escapePodView;

        private static readonly Color AllowedColor = new Color32(0x00, 0xFF, 0x00, 0x40);
        private static readonly Color NotAllowedColor = new Color32(0xFF, 0x00, 0x00, 0x40);
        private static readonly Color BlankColor = new Color32(0xFF, 0xFF, 0xFF, 0x00);
        private static readonly Color NotConstructed = new Color32(0xFF, 0xFF, 0x00, 0x80);

        private static readonly Color BuildableArea = new Color(1f, 1f, 0f, 0.5f);

        public void Start()
        {
            _escapePodView = EscapePod.GetComponent<EscapePodView>();
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
            for (var x = 0; x < _escapePodView.Columns; x++)
            {
                for (var y = 0; y < _escapePodView.Rows; y++)
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
                if (_escapePodView.GetStructureSlotView(x, y).IsNotConstructed()) return 2;

                //If has not - check if there is adjecent structure
                return HasAdjecentStructure(x, y) ? 0 : -1;
            }

            //If X/Y slot has any unconstructed structures
            if (_escapePodView.GetStructureSlotView(x, y).IsNotConstructed()) return 2;

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
            if (currentItem.ItemFunction == ItemFunction.Machine)
            {
                var itemBelow = GetStuctureItemFromElevation(x, y, elevation - 1);
                if (itemBelow == null) return 0;
                return itemBelow.ItemId == ItemId.BasicFloor ? 1 : 0;
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
                    var ss = _escapePodView.GetStructureSlotView(cX, cY);
                    if (ss != null && !ss.IsEmpty()) return true;
                }
            }
            return false;
        }

        private static bool IsEmpty(int x, int y, 
            StructureElevation structureElevation = StructureElevation.None)
        {
            return _escapePodView.GetStructureSlotView(x, y).IsEmpty(structureElevation);
        }

        private static Item GetStuctureItemFromElevation(int x, int y, StructureElevation elevation)
        {
            return _escapePodView.GetStructureSlotView(x, y).GetItem(elevation);
        }

        private static void SetBuildOverlayColor(int x, int y, Color color, string sprite)
        {
            var go = _escapePodView.StructureSlots[x,y];
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
            var success = _escapePodView.AddStructure(x, y, itemId, false);
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
            var spaceInInventory = InventoryController
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
            _escapePodView.RemoveStructure(x, y, elevation);
            InventoryController.AddItem(item.ItemId, 1);
            RefreshBuildOverlay(false);
        }

        private static void DebugShowBuilableArea()
        {
            for (var x = 0; x < _escapePodView.Columns; x++)
            {
                for (var y = 0; y < _escapePodView.Rows; y++)
                {
                    SetBuildOverlayColor(x, y, BuildableArea, "square");
                }
            }
        }

        #region INTEGRITY CHECK

        #region AIR CHECK
        private static SimpleTile[,] _airMap;

        private class SimpleTile
        {
            public int SelfX { get; set; }
            public int SelfY { get; set; }
            public SimpleTileType Type { get; set; }
            public int FloodGroup { get; set; }
            public bool HasAir { get; set; }
            public Vector2 AirLossDir { get; set; }
        }

        private enum SimpleTileType
        {
            Empty,
            Floor,
            Wall,
        }

        //Check which areas are fully enclosed and have air
        private static void EscapePodPressureCheck()
        {
            //Create simplified map
            _airMap = new SimpleTile[_escapePodView.Columns, _escapePodView.Rows];
            for (var x = 0; x < _escapePodView.Columns; x++)
            {
                for (var y = 0; y < _escapePodView.Rows; y++)
                {
                    var ssv = _escapePodView.GetStructureSlotView(x, y);
                    var groundItem = ssv.GetItem(StructureElevation.Ground);
                    if (groundItem != null && groundItem.ItemId == ItemId.BasicFloor)
                    {
                        _airMap[x, y] = new SimpleTile() {SelfX = x, SelfY = y, Type = SimpleTileType.Floor};
                    }
                    else if (groundItem != null && groundItem.ItemId == ItemId.BasicWall)
                    {
                        _airMap[x, y] = new SimpleTile() {SelfX = x, SelfY = y, Type = SimpleTileType.Wall};
                    }
                    else if (groundItem == null || groundItem.ItemId == ItemId.None)
                    {
                        _airMap[x, y] = new SimpleTile() {SelfX = x, SelfY = y, Type = SimpleTileType.Empty};
                    }
                }
            }

            //Go through all tiles and  flood them
            var floodGroup = 1;
            for (var x = 0; x < _escapePodView.Columns; x++)
            {
                for (var y = 0; y < _escapePodView.Rows; y++)
                {
                    var t = _airMap[x, y];
                    if (t.FloodGroup == 0 && t.Type == SimpleTileType.Floor)
                    {
                        FloorFlood(x, y, floodGroup, SimpleTileType.Floor);
                        floodGroup++;
                    }
                }
            }

            //Group floods
            var groups = new List<List<SimpleTile>>();
            for (var floodGroupIndex = 1; floodGroupIndex < floodGroup; floodGroupIndex++)
            {
                var g = new List<SimpleTile>();
                var groupHasAir = true;
                for (var x = 0; x < _escapePodView.Columns; x++)
                {
                    for (var y = 0; y < _escapePodView.Rows; y++)
                    {
                        if (_airMap[x, y].FloodGroup == floodGroupIndex)
                        {
                            var airLossDir = PressureCheck(x, y, floodGroupIndex);
                            _airMap[x, y].AirLossDir = airLossDir;
                            _airMap[x, y].HasAir = (airLossDir == Vector2.zero);
                            if (groupHasAir && airLossDir != Vector2.zero) groupHasAir = false;
                            g.Add(_airMap[x, y]);
                        }
                    }
                }

                //If group lost air mark all tiles
                if (groupHasAir == false)
                {
                    foreach (var simpleTile in g)
                    {
                        simpleTile.HasAir = false;
                    }
                }
            }

            //Debug output
            for (var x = 0; x < _escapePodView.Columns; x++)
            {
                for (var y = 0; y < _escapePodView.Rows; y++)
                {
                    var t = _airMap[x, y];

                    //Tile
                    var tileColor = Color.clear;
                    switch (t.Type)
                    {
                        case SimpleTileType.Empty:
                            tileColor = Color.white;
                            break;
                        case SimpleTileType.Floor:
                            tileColor = Color.grey;
                            break;
                        case SimpleTileType.Wall:
                            tileColor = Color.black;
                            break;
                    }
                    SetBuildOverlayColor(x, y, tileColor, "square");

                    //Flood
                    if (_airMap[x, y].FloodGroup > 0)
                    {
                        if (!_airMap[x, y].HasAir)
                        {
                            Log.Info(string.Format("AirLossDir [{0},{1}] = {2}", x, y, _airMap[x, y].AirLossDir));
                            SetBuildDebugOverlayColor(x, y, Color.red);
                            SetBuildDebugArrow(x, y, _airMap[x, y].AirLossDir);
                        }
                        else
                        {
                            SetBuildDebugOverlayColor(x, y, Color.green);
                            SetBuildDebugArrow(x, y, Vector2.zero);} 

                    }
                    else
                    {
                        SetBuildDebugOverlayColor(x, y, Color.clear);
                        SetBuildDebugArrow(x, y, Vector2.zero);
                    }
                }
            }
        }

        private static void FloorFlood(int x, int y, int fillWith, SimpleTileType searchFor)
        {
            if (x < 0 || x >= _escapePodView.Columns) return;
            if (y < 0 || y >= _escapePodView.Rows) return;

            if (_airMap[x, y].Type == searchFor && _airMap[x, y].FloodGroup == 0)
            {
                _airMap[x, y].FloodGroup = fillWith;
                FloorFlood(x + 1, y, fillWith, searchFor);
                FloorFlood(x, y + 1, fillWith, searchFor);
                FloorFlood(x - 1, y, fillWith, searchFor);
                FloorFlood(x, y - 1, fillWith, searchFor);
            }
        }

        private static Vector2 PressureCheck(int x, int y, int floodGroup)
        {
            for (var nX = x - 1; nX <= x + 1; nX++)
            {
                for (var nY = y - 1; nY <= y + 1; nY++)
                {
                    if (nX == x && nY == y) continue;
                    if (nX < 0 || nX > _escapePodView.Columns - 1 || nY < 0 || nY > _escapePodView.Rows - 1) continue;

                    if (_airMap[nX, nY].FloodGroup != floodGroup
                        && _airMap[nX, nY].Type != SimpleTileType.Floor
                        && _airMap[nX, nY].Type != SimpleTileType.Wall)
                    {
                        return new Vector2(nX - x, nY - y);
                    }
                }
            }

            return Vector2.zero;
        }

        private static void SetBuildDebugOverlayColor(int x, int y, Color color)
        {
            var go = _escapePodView.StructureSlots[x, y];
            var marker = go.transform.FindChild("DebugMarker");
            marker.GetComponent<SpriteRenderer>().color = color;
        }

        private static void SetBuildDebugArrow(int x, int y, Vector2 dirVector2)
        {
            var go = _escapePodView.StructureSlots[x, y];
            var marker = go.transform.FindChild("DebugArrow");

            if (dirVector2 == Vector2.zero)
            {
                marker.GetComponent<SpriteRenderer>().color = Color.clear;
                return;
            }
            
            marker.GetComponent<SpriteRenderer>().color = Color.blue;
            var dir = MathHelper.Vector2Angle(dirVector2);
            marker.rotation = Quaternion.AngleAxis(dir, Vector3.back);
        }
        #endregion

        #region FRACTURE CHECK
        //Check if EscapePod is as ONE Structure and it's not "in pieces"
        //Pass through x/y of possible new "gap" 
        private static int[,] _map;

        private static bool EscapePodStructureCheck(int? virtualX, int? virtualY, bool debugMode)
        {
            //Create simplified map
            _map = new int[_escapePodView.Columns, _escapePodView.Rows];
            var firstNotEmptyX = -1;
            var firstNotEmptyY = -1;

            for (var x = 0; x < _escapePodView.Columns; x++)
            {
                for (var y = 0; y < _escapePodView.Rows; y++)
                {
                    var isEmpty = _escapePodView.GetStructureSlotView(x, y).IsEmpty(StructureElevation.BelowGround);
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
            for (var x = 0; x < _escapePodView.Columns; x++)
            {
                for (var y = 0; y < _escapePodView.Rows; y++)
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
            if (x < 0 || x >= _escapePodView.Columns) return;
            if (y < 0 || y >= _escapePodView.Rows) return;

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

        #endregion
    }
}
