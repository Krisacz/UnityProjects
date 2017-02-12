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
            var fullName = string.Format("{0}_{1}", spritesheetName, index);
            if (Sprites.ContainsKey(fullName)) return Sprites[fullName];
            var spritePath = string.Format("{0}/{1}", "Sprites", spritesheetName);
            var spriteSheet = Resources.LoadAll(spritePath);
            //Index 0 is the whole big texture itself thats why i'm starting from 1 - first "frame"
            for (var i = 1; i < spriteSheet.Length; i++)
            {
                var sprite = spriteSheet[i] as Sprite;
                if (sprite != null) Sprites.Add(sprite.name, sprite);
            }

            var r = Sprites[fullName];
            if(r == null) Log.Error("SpriteDatabse", "Get",  
                "SpriteSheet: " + spritesheetName + " Index: " + index + " is NULL!");
            return r;
        }
    }
}
