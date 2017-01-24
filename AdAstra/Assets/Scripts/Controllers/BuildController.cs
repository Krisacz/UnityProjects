using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.Views;
using UnityEngine;

namespace Assets.Scripts.Controllers
{
    public class BuildController: MonoBehaviour
    {
        public GameObject EscapePod;

        private bool _isOn = false;
        public bool IsOn()
        {
            return _isOn;
        }

        public void BuildModeOn()
        {
            _isOn = true;
            var epv = EscapePod.GetComponent<EscapePodView>();
            for (var x = 0; x < epv.Columns; x++)
            {
                for (var y = 0; y < epv.Rows; y++)
                {
                    var ssGo = epv.StructureSlots[x, y];
                    if (ssGo.transform.childCount > 0)
                    {
                        ssGo.GetComponent<SpriteRenderer>().color = new Color32(0xFF, 0x00, 0x00, 0x50);
                    }
                    else
                    {
                        ssGo.GetComponent<SpriteRenderer>().color = new Color32(0x00, 0xFF, 0x00, 0x50);
                    }
                }
            }
        }

        public void BuildModeOff()
        {
            _isOn = false;
            var epv = EscapePod.GetComponent<EscapePodView>();
            for (var x = 0; x < epv.Columns; x++)
            {
                for (var y = 0; y < epv.Rows; y++)
                {
                    var ssGo = epv.StructureSlots[x, y];
                    ssGo.GetComponent<SpriteRenderer>().color = new Color32(0xFF, 0xFF, 0xFF, 0x00);
                }
            }
        }
    }
}
