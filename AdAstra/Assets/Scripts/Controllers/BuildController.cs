using System;
using System.Collections.Generic;
using System.Linq;
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

        private static readonly Color AllowedColor = new Color32(0x00, 0xFF, 0x00, 0x40);
        private static readonly Color NotAllowedColor = new Color32(0xFF, 0x00, 0x00, 0x40);
        private static readonly Color BlankColor = new Color32(0xFF, 0xFF, 0xFF, 0x00);
        private static readonly Color NotConstructed = new Color32(0xFF, 0xFF, 0x00, 0x80);

        private static readonly Color BuildableArea = new Color(1f, 1f, 0f, 0.5f);

        public void Start()
        {
            EscapePodView = EscapePod.GetComponent<EscapePodView>();
        }

        public void Update()
        {
            BuildModeToggle();
            DebugModeToggle();
        }

        private void BuildModeToggle()
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

        private void DebugModeToggle()
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
            RefreshBuildOverlay();
        }

        public static void RefreshBuildOverlay()
        {
            var blank = SpritesDatabase.Get("square");

            for (var x = 0; x < EscapePodView.Columns; x++)
            {
                for (var y = 0; y < EscapePodView.Rows; y++)
                {
                    var go = EscapePodView.StructureSlots[x, y];
                    var marker = go.transform.FindChild("Marker");
                    marker.GetComponent<SpriteRenderer>().sprite = blank;

                    var c = CanBuildInStructureSlot(x, y);
                    switch (c)
                    {
                        case -1:
                            SetBuildOverlayColor(x, y, BlankColor);
                            break;

                        case 0:
                            SetBuildOverlayColor(x, y, NotAllowedColor);
                            break;

                        case 1:
                            SetBuildOverlayColor(x, y, AllowedColor);
                            break;
                    }
                }
            }
        }

        //Return: -1 -> BLANK, 0 - NOT ALLOWED, 1 - ALLOWED
        public static int CanBuildInStructureSlot(int x, int y)
        {
            var currentItem = ItemSelectionController.GetItem();
            
            //If currently selected item is NOT structure type (or is NULL)
            if (currentItem == null 
                || !currentItem.FunctionProperties.ContainsKey(FunctionProperty.Elevation))
            {
                return HasAdjecentStructure(x, y) ? 0 : -1;
            }

            //If X/Y slot has any unconstructed structures
            if (EscapePodView.GetStructureSlotView(x, y).IsNotConstructed()) return 0;

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

        public static void RefreshNotContructedStructuresOverlay()
        {
            var empty = SpritesDatabase.Get("square");
            var notConstructed = SpritesDatabase.Get("not_constructed");

            for (var x = 0; x < EscapePodView.Columns; x++)
            {
                for (var y = 0; y < EscapePodView.Rows; y++)
                {
                    var go = EscapePodView.StructureSlots[x, y];
                    var ssv = go.GetComponent<StructureSlotView>();
                    var isNotConstructed = ssv.IsNotConstructed();
                    var marker = go.transform.FindChild("Marker");
                    var sprite = isNotConstructed ? notConstructed : empty;

                    marker.GetComponent<SpriteRenderer>().sprite = sprite;
                    SetBuildOverlayColor(x, y, isNotConstructed ? NotConstructed : BlankColor);
                }
            }
        }

        private static void SetBuildOverlayColor(int x, int y, Color color)
        {
            var go = EscapePodView.StructureSlots[x,y];
            var marker = go.transform.FindChild("Marker");
            marker.GetComponent<SpriteRenderer>().color = color;
        }

        public static void BuildModeOff()
        {
            _isOn = false;
            RefreshNotContructedStructuresOverlay();
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
                RefreshBuildOverlay();
            }
            else
            {
                //This should neve happen!
                Log.Error("BuildController", "Build",
                    string.Format("Strange! Somehow you are were unable to " +
                                  "build in here! [X:{0},Y:{1}]!", x, y));
            }
        }

        private static void DebugShowBuilableArea()
        {
            for (var x = 0; x < EscapePodView.Columns; x++)
            {
                for (var y = 0; y < EscapePodView.Rows; y++)
                {
                    SetBuildOverlayColor(x, y, BuildableArea);
                }
            }
        }
    }
}
