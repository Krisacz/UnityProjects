using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.Models;

namespace Assets.Scripts
{
    public static class DictionaryExtensions
    {
        public static float AsFloat(this Dictionary<FunctionProperty, string> d, FunctionProperty key)
        {
            if (!d.ContainsKey(key))
            {
                Log.Error("DictionaryExtensions", "AsFloat",
                    string.Format("Item FunctionProperties does not contain key: {0}", key));
            }

            return float.Parse(d[key]);
        }

        public static bool AsBool(this Dictionary<FunctionProperty, string> d, FunctionProperty key)
        {
            if (!d.ContainsKey(key))
            {
                Log.Error("DictionaryExtensions", "AsBool", 
                    string.Format("Item FunctionProperties does not contain key: {0}", key));
            }

            return bool.Parse(d[key]);
        }

        public static T AsEnum<T>(this Dictionary<FunctionProperty, string> d, FunctionProperty key)
        {
            if (!d.ContainsKey(key))
            {
                Log.Error("DictionaryExtensions", "AsEnum",
                    string.Format("Item FunctionProperties does not contain key: {0}", key));
            }

            return (T)Enum.Parse(typeof(T), d[key]);
        }
    }
}
