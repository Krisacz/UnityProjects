using System;
using System.Collections.Generic;
using Assets.Scripts.Db;
using Assets.Scripts.Models;
using UnityEngine;

namespace Assets.Scripts.Views
{
    public class EscapePodView : MonoBehaviour
    {
        public readonly int Columns = 31;
        public readonly int Rows = 31;
        public GameObject[,] StructureSlots;

        // Use this for initialization
        void Start ()
        {
            StructureSlots = new GameObject[Columns, Rows];
            Init();
            CreateInitialEscapePod();
            this.transform.position = new Vector3(-15f, -15f, 0f);
        }

        private void Init()
        {
            for (var x = 0; x < Columns; x++)
            {
                for (var y = 0; y < Rows; y++)
                {
                    StructureSlots[x, y] = GameObjectFactory.StructureSlot(x, y, this.transform);
                }
            }
        }

        private void CreateInitialEscapePod()
        {
            //Foundations & walls
            for (var x = 0; x < Columns; x++)
            {
                for (var y = 0; y < Rows; y++)
                {
                    //Foundation
                    if (x >= 12 && x <= 18 && y >= 12 && y <= 18)
                    {
                        AddStructure(x, y, ItemId.BasicFoundation, true);

                        //Walls
                        if (x == 12 || x == 18 || y == 12 || y == 18)
                        {
                            //Except this one...
                            if (x == 15 && y == 12)
                            {
                                AddStructure(x, y, ItemId.BasicFloor, true);
                                continue;
                            }

                            AddStructure(x, y, ItemId.BasicWall, true);
                        }
                        else
                        {
                            AddStructure(x, y, ItemId.BasicFloor, true);
                        }
                    }
                }
            }
        }

        public bool AddStructure(int x, int y, ItemId itemId, bool instaBuild)
        {
            var item = ItemsDatabase.Get(itemId);
            if (!item.FunctionProperties.ContainsKey(FunctionProperty.Elevation))
            {
                Log.Error("EscapePodView", "AddStructure", 
                    string.Format("You are trying to build with item without elevation property." +
                                  " ItemId = {0}", itemId));
                return false;
            }

            //Elevation and position
            var elevation = item.FunctionProperties.AsEnum<StructureElevation>(FunctionProperty.Elevation);
            var structureSlotGo = StructureSlots[x, y];
            var structureSlotView = structureSlotGo.GetComponent<StructureSlotView>();

            //Interact UI Type
            var interactUIType = InteractUIType.None;
            if (item.ItemFunction == ItemFunction.Machine)
            {
                interactUIType = item.FunctionProperties.AsEnum<InteractUIType>(FunctionProperty.InteractUIType);
            }

            return structureSlotView.AddStructure(elevation, itemId, interactUIType, instaBuild);
        }

        public StructureSlotView GetStructureSlotView(int x, int y)
        {
            if (x < 0 || x > Columns - 1 || y < 0 || y > Rows - 1) return null;
            return StructureSlots[x, y].GetComponent<StructureSlotView>();
        }

        public void RemoveStructure(int x, int y, StructureElevation elevation)
        {
            var structureSlotGo = StructureSlots[x, y];
            var structureSlotView = structureSlotGo.GetComponent<StructureSlotView>();
            structureSlotView.RemoveStructure(elevation);
        }

        //TODO Do we need this?
        public List<GameObject> GetNeighbors(int x, int y)
        {
            var list = new List<GameObject>();
            for (var nX = x - 1; nX <= x + 1; nX++)
            {
                for (var nY = y - 1; nY <= y + 1; nY++)
                {
                    //Skip if same tile or out of bounds
                    if(nX == x && nY == y) continue;
                    if(nX < 0 || nX >= Columns || nY < 0 || nY >= Rows) continue;
                    list.Add(StructureSlots[nX, nY]);
                }
            }

            return list;
        }
    }
}
