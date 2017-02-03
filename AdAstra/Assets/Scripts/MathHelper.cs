using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts
{
    public static class MathHelper
    {
        //--------------------------------------------------
        //                   0°[0,1]                        
        //                     |                            
        //         315°[-1,1]\ | /45°[1,1]                  
        //                    \|/                           
        //       270°[-1,0]----@----90°[1,0]                
        //                    /|\                           
        //        225°[-1,-1]/ | \135°[1,-1]                
        //                     |                            
        //                 180°[0,-1]                       
        //--------------------------------------------------
        //The diagonal angles are returning values 0.7 not 1 


        public static Vector2 Angle2Vector(float a)
        {
            var rad = DegreeToRadian(a);
            return new Vector2((float) Math.Sin(rad), (float) Math.Cos(rad));
        }

        public static float Vector2Angle(Vector2 v)
        {
            if (v.x < 0) return 360 - (Mathf.Atan2(v.x, v.y) * Mathf.Rad2Deg * -1);
            return Mathf.Atan2(v.x, v.y) * Mathf.Rad2Deg;
        }

        public static float DegreeToRadian(float a)
        {
            return a * Mathf.Deg2Rad;
        }
    }
}
