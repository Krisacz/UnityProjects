using System;
using UnityEngine;

namespace Assets.Scripts.Models
{
    public class Asteroid: IComparable<Asteroid>
    {
        public GameObject GameObject { get; set; }
        public int SpriteId { get; set; }
        public AsteroidSize AsteroidSize { get; set; }
        public float Scale { get; set; }
        public int NodesCount { get; set; }
        public int InitialDistance { get; set; }

        public int CompareTo(Asteroid other)
        {
            if (InitialDistance == other.InitialDistance) return  0;
            if (InitialDistance <  other.InitialDistance) return -1;
            if (InitialDistance >  other.InitialDistance) return  1;
            return -1;
        }
    }
}