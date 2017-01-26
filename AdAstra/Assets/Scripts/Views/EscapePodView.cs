using System;
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
                        AddStructure(x, y, StructureElevation.Ground, ItemId.Foundation, true);

                        //Walls
                        if (x == 12 || x == 18 || y == 12 || y == 18)
                        {
                            //Except this one...
                            if (x == 15 && y == 12)
                            {
                                AddStructure(x, y, StructureElevation.OnGround, ItemId.Floor, true);
                                continue;
                            }

                            AddStructure(x, y, StructureElevation.OnGround, ItemId.Wall, true);
                        }
                        else
                        {
                            AddStructure(x, y, StructureElevation.OnGround, ItemId.Floor, true);
                        }
                    }
                }
            }
        }

        public bool AddStructure(int x, int y, StructureElevation elevation, 
            ItemId itemId, bool instaBuild)
        {
            var structureSlotGo = StructureSlots[x, y];
            var structureSlotView = structureSlotGo.GetComponent<StructureSlotView>();
            return structureSlotView.AddStructure(elevation, itemId, instaBuild);
        }

        public StructureSlotView GetStructureSlotView(int x, int y)
        {
            if (x < 0 || x > Columns - 1 || y < 0 || y > Rows - 1) return null;
            return StructureSlots[x, y].GetComponent<StructureSlotView>();
        }

        // Update is called once per frame
        void Update()
        {
        }
    }
}
