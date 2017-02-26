using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Random = UnityEngine.Random;

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

        public static bool PointInsidePolygon(Vector2 point, PolygonCollider2D polygonCollider)
        {
            return polygonCollider.OverlapPoint(point);
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

        public static int RandomRange(float[] probs)
        {
            var total = 0.0f;
            foreach (var elem in probs) total += elem;
            var randomPoint = Random.value * total;
            for (var i = 0; i < probs.Length; i++)
            {
                if (randomPoint < probs[i]) return i;
                randomPoint -= probs[i];
            }
            return probs.Length - 1;
        }

        public static bool PointInsideCircle(Vector2 position, CircleCollider2D circle)
        {
            return circle.OverlapPoint(position);
        }

        public static bool PointsInsideRect(IEnumerable<Vector2> points, Bounds rect)
        {
            foreach (var point in points)
            {
                var inside = rect.Contains(point);
                if (!inside) return false;
            }
            return true;
        }

        public static Vector3 RandomPointInBounds(Bounds bounds)
        {
            var center = bounds.center;
            var x = Random.Range(center.x - bounds.extents.x, center.x + bounds.extents.x);
            var y = Random.Range(center.y - bounds.extents.y, center.y + bounds.extents.y);
            return new Vector3(x, y, 0f);
        }

        public static Vector2 RandomInCircle(Vector2 center, float radious)
        {
            var rnd = Random.insideUnitCircle * radious;
            return new Vector2(center.x + rnd.x, center.y + rnd.y);
        }

        public static bool RandomBool()
        {
            return Random.Range(1, 101) > 51;
        }

        public static Vector3 GetMouseToWorldPosition()
        {
            var input = Input.mousePosition;
            var worldPosition = Camera.main.ScreenToWorldPoint(input);
            worldPosition.z = 0f;
            return worldPosition;
        }
    }
}
