using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Db;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.Scripts.Controllers
{
    public class AsteroidInfo
    {
        public List<int> SpriteIds { get; set; }
        public AsteroidSize AsteroidSize { get; set; }
        public List<AsteroidSizeToOreNodes> SizeToOreNodes { get; set; }
    }
    
    public class AsteroidSizeToOreNodes
    {
        public float MinScale { get; set; }
        public float MaxScale { get; set; }
        public int MaxOreNodes { get; set; }
    }

    public enum AsteroidSize { Tiny, Small, Medium, Large }

    public class AsteroidController : MonoBehaviour
    {
        #region ASTEROID INFOS
        private readonly List<AsteroidInfo> _infos = new List<AsteroidInfo>()
        {
            new AsteroidInfo()
            {
                AsteroidSize = AsteroidSize.Large,
                SpriteIds = new List<int>() { 0, 1, 2, 3, 4, 31 },
                SizeToOreNodes = new List<AsteroidSizeToOreNodes>()
                {
                    new AsteroidSizeToOreNodes() { MinScale = 0.25f, MaxScale = 0.75f, MaxOreNodes = 0 },
                    new AsteroidSizeToOreNodes() { MinScale = 1.00f, MaxScale = 1.75f, MaxOreNodes = 2 },
                    new AsteroidSizeToOreNodes() { MinScale = 2.00f, MaxScale = 3.00f, MaxOreNodes = 4 },
                    new AsteroidSizeToOreNodes() { MinScale = 3.25f, MaxScale = 4.00f, MaxOreNodes = 6 },
                    new AsteroidSizeToOreNodes() { MinScale = 4.25f, MaxScale = 5.00f, MaxOreNodes = 7 },
                    new AsteroidSizeToOreNodes() { MinScale = 4.25f, MaxScale = 5.00f, MaxOreNodes = 8 },
                }
            },

            new AsteroidInfo()
            {
                AsteroidSize = AsteroidSize.Medium,
                SpriteIds = new List<int>() { 5, 10, 12, 13, 15, 16, 17, 18, 19,
                                                        20, 21, 22, 23, 24, 32, 33, 34 },
                SizeToOreNodes = new List<AsteroidSizeToOreNodes>()
                {
                    new AsteroidSizeToOreNodes() { MinScale = 0.25f, MaxScale = 0.75f, MaxOreNodes = 0 },
                    new AsteroidSizeToOreNodes() { MinScale = 1.00f, MaxScale = 1.00f, MaxOreNodes = 1 },
                    new AsteroidSizeToOreNodes() { MinScale = 1.25f, MaxScale = 2.00f, MaxOreNodes = 2 },
                    new AsteroidSizeToOreNodes() { MinScale = 2.25f, MaxScale = 3.00f, MaxOreNodes = 3 },
                    new AsteroidSizeToOreNodes() { MinScale = 3.25f, MaxScale = 4.00f, MaxOreNodes = 4 },
                    new AsteroidSizeToOreNodes() { MinScale = 4.25f, MaxScale = 5.00f, MaxOreNodes = 5 },
                }
            },

            new AsteroidInfo()
            {
                AsteroidSize = AsteroidSize.Small,
                SpriteIds = new List<int>() { 8, 9, 25, 26, 27, 28, 29, 30, 36 },
                SizeToOreNodes = new List<AsteroidSizeToOreNodes>()
                {
                    new AsteroidSizeToOreNodes() { MinScale = 0.25f, MaxScale = 1.00f, MaxOreNodes = 0 },
                    new AsteroidSizeToOreNodes() { MinScale = 1.25f, MaxScale = 2.00f, MaxOreNodes = 1 },
                }
            },

            new AsteroidInfo()
            {
                AsteroidSize = AsteroidSize.Tiny,
                SpriteIds = new List<int>() { 6, 35, 7 },
                SizeToOreNodes = new List<AsteroidSizeToOreNodes>()
                {
                    new AsteroidSizeToOreNodes() { MinScale = 0.50f, MaxScale = 2.00f, MaxOreNodes = 0 },
                }
            }
        };
        #endregion

        private int min = 1;
        private int max = 1;
        private int repeat = 1;

        void OnGUI()
        {
            GUI.color = Color.green;

            //Min slider
            GUI.Label(new Rect(5, 255, 100, 20), "Min: [" + min + "]");
            min = (int)GUI.HorizontalSlider(new Rect(5, 275, 100, 30), min, 1.0F, 8.0F);

            //Max slider
            GUI.Label(new Rect(5, 300, 100, 20), "Max: [" + max + "]");
            max = (int)GUI.HorizontalSlider(new Rect(5, 320, 100, 30), max, 1.0F, 8.0F);

            //Labels
            //GUI.Label(new Rect(5, 350, 250, 20), "Selected Sprite ID #" + selectedSpriteId);
            //GUI.Label(new Rect(5, 375, 250, 20), "Selected Sprite Sc #" + selectedScale.ToString("F2"));
            //GUI.Label(new Rect(5, 400, 250, 20), "Emergency Exit Cnt #" + emergencyExistCount);

            //Button
            GUI.Label(new Rect(10, 150, 100, 20), "Repeat: [" + repeat + "]");
            repeat = (int)GUI.HorizontalSlider(new Rect(10, 170, 100, 30), repeat, 1.0F, 10.0F);
            if (GUI.Button(new Rect(10, 200, 100, 50), "Spawn"))
            {
                for (int i = 0; i < repeat; i++)
                {
                    foreach (var gameObj in FindObjectsOfType<GameObject>())
                    {
                        if (gameObj.name.Equals("Asteroid")) Destroy(gameObj);
                    }

                    
                    SpawnAsteroid(min, max);
                }
            }

            if (GUI.Button(new Rect(10, 400, 100, 50), "XXX"))
            {
                var m = -1f;
                for (int i = 0; i < 10000; i++)
                {
                    var r = Random.Range(0f, 1f);
                    m = Mathf.Max(m, r);
                }
                Log.Info("Max: " + m);
            }
        }

        public void SpawnAsteroid(int minOreNodes, int maxOreNodes)
        {
            var asteroidInfo = GetRandomAsteroidInfoFromOreNodeCount(minOreNodes, maxOreNodes);
            var asteroidGo = GetRandomAsteroidFromAsteroidsInfo(asteroidInfo);
            asteroidGo.transform.position = Vector3.zero;
        }

        private AsteroidInfo GetRandomAsteroidInfoFromOreNodeCount(int minCount, int maxCount)
        {
            if (minCount < 0 || minCount > 8)
            {
                Log.Error("AsteroidController", 
                    "GetRandomAsteroidInfoFromOreNodeCount",
                    "MinCount Count needs to be between 0 and 8, provided: " + minCount);
                return null;
            }

            if (maxCount < 0 || maxCount > 8)
            { Log.Error("AsteroidController",
                "GetRandomAsteroidInfoFromOreNodeCount",
                "Max Count needs to be between 0 and 8, provided: " + maxCount);
                return null;
            }

            if (minCount > maxCount)
            {
                Log.Error("AsteroidController",
                    "GetRandomAsteroidInfoFromOreNodeCount",
                    "MinCount needs to be less or equal to MaxCount, Min: " + minCount + ", Max:" + maxCount);
                return null;
            }

            var available = GetAsteroidsInfoForOreNodeCount(minCount, maxCount);
            
            //In case we've somehow got no results
            if (available.Count == 0) Log.Error("AsteroidController",
                "GetRandomAsteroidInfoFromOreNodeCount", "No results?!?! ExactCount: " + minCount);

            //Randomize index from available asteroids
            var rndIndex = Random.Range(0, available.Count);

            //Return
            return available[rndIndex];
        }

        private List<AsteroidInfo> GetAsteroidsInfoForOreNodeCount(int minCount, int maxCount)
        {
            if (minCount < 0 || minCount > 8) Log.Error("AsteroidController",
                "GetAsteroidsInfoForOreNodeCount", "Min Count needs to be between 0 and 8, provided: " + minCount);

            if (maxCount < 0 || maxCount > 8) Log.Error("AsteroidController",
                "GetAsteroidsInfoForOreNodeCount", "Max Count needs to be between 0 and 8, provided: " + maxCount);

            var list = new List<AsteroidInfo>();

            //Loop through all data and find those asterids which fits our criteria
            foreach (var asteroidInfo in _infos)
            {
                var ai = new AsteroidInfo
                {
                    AsteroidSize = asteroidInfo.AsteroidSize,
                    SpriteIds = asteroidInfo.SpriteIds,
                    SizeToOreNodes = new List<AsteroidSizeToOreNodes>()
                };

                foreach (var a in asteroidInfo.SizeToOreNodes)
                {
                    if (a.MaxOreNodes > 0 && a.MaxOreNodes <= maxCount && a.MaxOreNodes >= minCount)
                    {
                        ai.SizeToOreNodes.Add(a);
                    }
                }

                if(ai.SizeToOreNodes.Count > 0) list.Add(ai);
            }

            return list;
        }

        private GameObject GetRandomAsteroidFromAsteroidsInfo(AsteroidInfo asteroidInfo)
        {
            var randomSpriteId = asteroidInfo.SpriteIds[Random.Range(0, asteroidInfo.SpriteIds.Count)];
            var randomSizeToOreNodes = asteroidInfo.SizeToOreNodes[Random.Range(0, 
                asteroidInfo.SizeToOreNodes.Count)];
            var randomScale = Random.Range(randomSizeToOreNodes.MinScale, randomSizeToOreNodes.MaxScale);
            var asteroidGo = GameObjectFactory.Asteroid(randomSpriteId, randomScale, this.transform);
            asteroidGo.transform.position = Vector3.zero;
            PlaceRandomOreNodesInsideAsteroid(asteroidGo, randomSizeToOreNodes.MaxOreNodes);
            return asteroidGo;
        }

        private void PlaceRandomOreNodesInsideAsteroid(GameObject asteroidGo, int maxOreNodes)
        {
            var oreNodesGo = new List<GameObject>();
            for (var i = 0; i < maxOreNodes; i++)
            {
                var oreNodeGo = GameObjectFactory.OreNode(asteroidGo.transform);
                PlaceOreNodeInsideAsteroid(asteroidGo, oreNodeGo, oreNodesGo);
                oreNodesGo.Add(oreNodeGo);
            }
        }

        private void PlaceOreNodeInsideAsteroid(GameObject asteroid, 
            GameObject oreNode, List<GameObject> oreNodesGo)
        {
            Log.Info("ID: " + asteroid.GetComponent<SpriteRenderer>().sprite.name);
            var polygon = asteroid.GetComponent<PolygonCollider2D>();
            var astBounds = polygon.bounds;
            var isOreNodeInside = false;
            var oreNodeSr = oreNode.GetComponent<SpriteRenderer>();
            var oreNodeBounds = new Bounds(oreNodeSr.bounds.center, oreNodeSr.bounds.size);
            var rndPoint = Vector3.zero;
            var emergencyExit = 10000;
            do
            {
                emergencyExit--;
                if(emergencyExit <= 0) break;

                rndPoint = new Vector2(Random.Range(astBounds.min.x, astBounds.max.x),
                    Random.Range(astBounds.min.y, astBounds.max.y));
                oreNodeBounds.center = rndPoint;
                var corners = MathHelper.GetSquareCorners(oreNodeBounds);
                isOreNodeInside = MathHelper.PointsInsidePolygon(corners, polygon);

                //If center and corners areinside asteroid check if it doesn't overlap with other ore nodes
                if (!isOreNodeInside) continue;
                foreach (var o in oreNodesGo)
                {
                    var existing = o.transform.GetComponent<SpriteRenderer>().bounds;
                    if (existing.Intersects(oreNodeBounds))
                    {
                        isOreNodeInside = false;
                        break;
                    }
                }
            }
            while (!isOreNodeInside);

            oreNodeSr.transform.position = rndPoint;
        }
    }
}
