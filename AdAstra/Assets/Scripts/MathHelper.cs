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

        public static bool PointsInsidePolygon(IEnumerable<Vector2> points, PolygonCollider2D polygonCollider)
        {
            foreach (var point in points)
            {
                var inside = polygonCollider.OverlapPoint(point);
                if (!inside) return false;
            }
            return true;
        }

        public static Vector2[] GetSquareCorners(Bounds bounds)
        {
            var topLeft = new Vector2(bounds.min.x, bounds.min.y);
            var topRight = new Vector2(bounds.min.x + bounds.size.x, bounds.min.y);
            var bottomLeft = new Vector2(bounds.min.x, bounds.min.y + bounds.size.y);
            var bottomRight = new Vector2(bounds.min.x + bounds.size.x, bounds.min.y + bounds.size.y);
            return new[] {topLeft, topRight, bottomLeft, bottomRight};
        }
   
    }
}
