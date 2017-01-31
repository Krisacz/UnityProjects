using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Models
{
    public class Notification
    {
        public enum Mode
        {
            Wait,
            Appear,
            Show,
            Disappear,
            Destroy
        }

        public GameObject GameObj { get; set; }
        public Mode State { get; set; }
        public float ShowDuration { get; set; }
    }
}
