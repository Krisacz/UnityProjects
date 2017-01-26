using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        public void Start()
        {
            EscapePodView = EscapePod.GetComponent<EscapePodView>();
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
            for (var x = 0; x < EscapePodView.Columns; x++)
            {
                for (var y = 0; y < EscapePodView.Rows; y++)
                {
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
            var currentItem = ItemSelectionController.GetSelectedSlot().HasItem
                ? ItemSelectionController.GetSelectedSlot().GetItemStack().Item
                : null;

            if (IsEmpty(x, y))
            {
                if (HasAdjecentStructure(x, y))
                {
                    return currentItem != null && currentItem.StructureElevation == StructureElevation.Ground ? 1 : 0;
                }
                else
                {
                    return -1;
                }
            }
            else
            {
                return currentItem != null && IsEmpty(x, y, currentItem.StructureElevation) ? 1 : 0;
            }
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

        private static bool IsEmpty(int x, int y, StructureElevation structureElevation = StructureElevation.None)
        {
            return EscapePodView.GetStructureSlotView(x, y).IsEmpty(structureElevation);
        }

        public static void RefreshNotContructedStructuresOverlay()
        {
            for (var x = 0; x < EscapePodView.Columns; x++)
            {
                for (var y = 0; y < EscapePodView.Rows; y++)
                {
                    var ss = EscapePodView.GetStructureSlotView(x, y);
                    SetBuildOverlayColor(x, y, ss.IsNotConstructed() ? NotConstructed : BlankColor);
                }
            }
        }

        private static void SetBuildOverlayColor(int x, int y, Color color)
        {
            EscapePodView.StructureSlots[x,y].GetComponent<SpriteRenderer>().color = color;
        }

        public static void BuildModeOff()
        {
            _isOn = false;
            RefreshNotContructedStructuresOverlay();
        }

        public static void Build(int x, int y)
        {
            var currentItem = ItemSelectionController.GetSelectedSlot().HasItem
                ? ItemSelectionController.GetSelectedSlot().GetItemStack().Item
                : null;

            if (currentItem == null)
            {
                //This should neve happen!
                Log.Error("BuildController", "Build",
                    string.Format("Strange! Somehow you are " +
                                  "trying to build with NULL item! [X:{0},Y:{1}]!", x, y));
                return;
            }

            //Build!
            var elevation = currentItem.StructureElevation;
            var itemId = currentItem.ItemId;
            var success = EscapePodView.AddStructure(x, y, elevation, itemId, false);
            if (success)
            {
                ItemSelectionController.GetSelectedSlot().UpdateStackCount(-1);
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
    }
}
