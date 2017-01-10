using System;
using Assets.Scripts.Models;
using UnityEngine;

namespace Assets.Scripts.Views
{
    public class EscpaePodView : MonoBehaviour
    {
        private const int Columns = 10;
        private const int Rows = 10;
        private GameObject[,] _structureSlots;

        // Use this for initialization
        void Start ()
        {
            _structureSlots = new GameObject[Columns, Rows];
            Init();
            CreateInitialEscapePod();
        }

        private void Init()
        {
            for (var x = 0; x < Columns; x++)
            {
                for (var y = 0; y < Rows; y++)
                {
                    var go = GameObjectFactory.FromPrefab("StructureSlot");
                    go.name = string.Format("StructureSlot X[{0}]-Y[{1}]", x, y);
                    go.transform.position = new Vector3(x, y);
                    go.transform.parent = this.transform;
                    _structureSlots[x, y] = go;
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
                    var structureSlotView = _structureSlots[x, y].GetComponent<StructureSlotView>();
                    structureSlotView.AddStructure(StructureElevation.Ground, ItemId.Foundation);

                    if (x == 0 || x == Columns - 1 || y == 0 || y == Rows - 1)
                    {
                        if (x == 0 && (y == 4 || y == 5)) continue;
                        structureSlotView.AddStructure(StructureElevation.OnGround, ItemId.Wall);
                    }
                }
            }
        }

        // Update is called once per frame
        void Update()
        {
        }
    }
}
