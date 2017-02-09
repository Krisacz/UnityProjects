using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Db
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

        //Get existing or load and add to database - load only once!
        public static Sprite Get(string spritesheetName, int index)
        {
            var indexP1 = index + 1;
            var fullName = string.Format("{0}_{1}", spritesheetName, indexP1);
            if (Sprites.ContainsKey(fullName)) return Sprites[fullName];
            var spritePath = string.Format("{0}/{1}", "Sprites", spritesheetName);
            var spriteSheet = Resources.LoadAll(spritePath);
            for (var i = 1; i < spriteSheet.Length; i++)
            {
                var sprite = spriteSheet[i] as Sprite;
                var name = string.Format("{0}_{1}", spritesheetName, i);
                Sprites.Add(name, sprite);
            }

            var r = Sprites[fullName];
            Log.Error("SpriteDatabse", "Get",  "SpriteSheet: " + spritesheetName + " Index: " + indexP1 + " is NULL!");
            return r;
        }
    }
}
