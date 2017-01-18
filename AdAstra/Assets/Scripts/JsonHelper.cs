using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LitJson;
using UnityEngine.Networking;

namespace Assets.Scripts
{
    public static class JsonHelper
    {
        public static string AsString(IEnumerable data)
        {
            return data.ToString();
        }

        public static int AsInt(IEnumerable data)
        {
            return int.Parse(data.ToString());
        }

        public static float AsFloat(IEnumerable data)
        {
            return float.Parse(data.ToString());
        }

        public static double AsDouble(IEnumerable data)
        {
            return double.Parse(data.ToString());
        }

        public static T AsEnum<T>(IEnumerable data)
        {
            return (T)Enum.Parse(typeof(T), AsString(data));
        }

        public static Dictionary<T1, T2> AsDictionary<T1,T2>(string data)
        {
            return JsonMapper.ToObject<Dictionary<T1, T2>>(data);
        } 
    }
}
