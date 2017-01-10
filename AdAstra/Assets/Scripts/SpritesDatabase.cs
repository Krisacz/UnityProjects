using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts
{
    public static class SpritesDatabase
    {
        private static readonly Dictionary<string, Sprite> Sprites = new Dictionary<string, Sprite>();

        //Get existing or load and add to database - load only once!
        public static Sprite Get(string spriteName)
        {
            if (Sprites.ContainsKey(spriteName)) return Sprites[spriteName];
            var spritePath = string.Format("{0}/{1}", "Sprites", spriteName);
            var sprite = Resources.Load(spritePath, typeof(Sprite)) as Sprite;
            Sprites.Add(spriteName, sprite);
            return sprite;
        }
    }
}
