﻿using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts
{
    public class Curver : MonoBehaviour
    {
        public Transform[] Points;
    
        // Use this for initialization
        void Start ()
        {
            Refresh();
        }

        public void Refresh()
        {
            var c1 = Color.yellow;
            var c2 = Color.red;
            var lr = GetComponent<LineRenderer>();
            var points = new List<Vector3>(Points.Select(x=>x.position));
            //Points = CurverController.MakeSmoothCurve(points.ToArray(), 3.0f);
            var pointes = CurverController.MakeSmoothCurve(points.ToArray(), 3.0f);
            lr.startColor = c1;
            lr.endColor = c2;
            lr.startWidth = 0.5f;
            lr.endWidth = 0.5f;
            lr.numPositions = Points.Length;

            for (int index = 0; index < Points.Length; index++)
            {
                var point = points[index];
                lr.SetPosition(index, point);
            }
        }
    
        public class CurverController : MonoBehaviour
        {
            //arrayToCurve is original Vector3 array, smoothness is the number of interpolations. 
            public static Vector3[] MakeSmoothCurve(Vector3[] arrayToCurve, float smoothness)
            {
                List<Vector3> points;
                List<Vector3> curvedPoints;
                int pointsLength = 0;
                int curvedLength = 0;

                if (smoothness < 1.0f) smoothness = 1.0f;

                pointsLength = arrayToCurve.Length;

                curvedLength = (pointsLength * Mathf.RoundToInt(smoothness)) - 1;
                curvedPoints = new List<Vector3>(curvedLength);

                float t = 0.0f;
                for (int pointInTimeOnCurve = 0; pointInTimeOnCurve < curvedLength + 1; pointInTimeOnCurve++)
                {
                    t = Mathf.InverseLerp(0, curvedLength, pointInTimeOnCurve);

                    points = new List<Vector3>(arrayToCurve);

                    for (int j = pointsLength - 1; j > 0; j--)
                    {
                        for (int i = 0; i < j; i++)
                        {
                            points[i] = (1 - t) * points[i] + t * points[i + 1];
                        }
                    }
                 
                    curvedPoints.Add(points[0]);
                }

                return (curvedPoints.ToArray());
            }
        }
    }
}
