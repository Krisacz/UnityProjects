using System;
using System.Collections.Generic;
using System.Linq;
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
                    new AsteroidSizeToOreNodes() { MinScale = 1f, MaxScale = 1.75f, MaxOreNodes = 2 },
                    new AsteroidSizeToOreNodes() { MinScale = 2f, MaxScale = 3f, MaxOreNodes = 4 },
                    new AsteroidSizeToOreNodes() { MinScale = 3.25f, MaxScale = 4f, MaxOreNodes = 6 },
                    new AsteroidSizeToOreNodes() { MinScale = 4.25f, MaxScale = 5f, MaxOreNodes = 7 },
                    new AsteroidSizeToOreNodes() { MinScale = 4.25f, MaxScale = 5f, MaxOreNodes = 8 },
                }
            },

            new AsteroidInfo()
            {
                AsteroidSize = AsteroidSize.Medium,
                SpriteIds = new List<int>() { 5, 10, 12, 13, 15, 16, 17, 18, 19,
                                                        20, 21, 22, 23, 24, 32, 33, 34 },
                SizeToOreNodes = new List<AsteroidSizeToOreNodes>()
                {
                    new AsteroidSizeToOreNodes() { MinScale = 0.25f, MaxScale = 1f, MaxOreNodes = 1 },
                    new AsteroidSizeToOreNodes() { MinScale = 1.25f, MaxScale = 2f, MaxOreNodes = 2 },
                    new AsteroidSizeToOreNodes() { MinScale = 2.25f, MaxScale = 3f, MaxOreNodes = 3 },
                    new AsteroidSizeToOreNodes() { MinScale = 3.25f, MaxScale = 4f, MaxOreNodes = 4 },
                    new AsteroidSizeToOreNodes() { MinScale = 4.25f, MaxScale = 5f, MaxOreNodes = 5 },
                }
            },

            new AsteroidInfo()
            {
                AsteroidSize = AsteroidSize.Small,
                SpriteIds = new List<int>() { 8, 9, 25, 26, 27, 28, 29, 30, 36 },
                SizeToOreNodes = new List<AsteroidSizeToOreNodes>()
                {
                    new AsteroidSizeToOreNodes() { MinScale = 0.25f, MaxScale = 1f, MaxOreNodes = 0 },
                    new AsteroidSizeToOreNodes() { MinScale = 1.25f, MaxScale = 2f, MaxOreNodes = 1 },
                }
            },

            new AsteroidInfo()
            {
                AsteroidSize = AsteroidSize.Tiny,
                SpriteIds = new List<int>() { 6, 35, 7 },
                SizeToOreNodes = new List<AsteroidSizeToOreNodes>()
                {
                    new AsteroidSizeToOreNodes() { MinScale = 0.5f, MaxScale = 2f, MaxOreNodes = 0 },
                }
            }
        };
        #endregion

        void OnGUI()
        {
            if (GUI.Button(new Rect(10, 200, 150, 50), "Do stuff"))
            {
                SpawnAsteroid(3, 5);
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

            var available = GetAsteroidsInfoForOreNodeCount(minCount, minCount);

            //In case we hit one to the unavailable numbers - decrease criteria
            if (available.Count == 0) available = GetAsteroidsInfoForOreNodeCount(minCount - 1, minCount);

            //In case we've somehow got no results
            if (available.Count == 0) Log.Error("AsteroidController",
                "GetRandomAsteroidInfoFromOreNodeCount", "No results?!?! ExactCount: " + minCount);

            //Randomize index from available asteroids
            var rndIndex = Random.Range(0, available.Count - 1);

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
                if (asteroidInfo.SizeToOreNodes.Any(x => x.MaxOreNodes >= minCount && x.MaxOreNodes <= minCount))
                {
                    var ai = new AsteroidInfo
                    {
                        AsteroidSize = asteroidInfo.AsteroidSize,
                        SpriteIds = asteroidInfo.SpriteIds,
                        SizeToOreNodes = new List<AsteroidSizeToOreNodes>()
                    };

                    foreach (var x in asteroidInfo.SizeToOreNodes
                        .Where(x => x.MaxOreNodes >= minCount && x.MaxOreNodes <= maxCount))
                    {
                        ai.SizeToOreNodes.Add(x);
                    }

                    list.Add(ai);
                }
            }

            return list;
        }

        private GameObject GetRandomAsteroidFromAsteroidsInfo(AsteroidInfo asteroidInfo)
        {
            var randomSpriteId = asteroidInfo.SpriteIds[Random.Range(0, asteroidInfo.SpriteIds.Count - 1)];
            var randomSizeToOreNodes = asteroidInfo.SizeToOreNodes[Random.Range(0, 
                asteroidInfo.SizeToOreNodes.Count - 1)];
            var randomScale = Random.Range(randomSizeToOreNodes.MinScale, randomSizeToOreNodes.MaxScale);
            var asteroidGo = GameObjectFactory.Asteroid(randomSpriteId, randomScale, this.transform);
            //TODO remove this
            //return asteroidGo;
            PlaceRandomOreNodesInsideAsteroid(asteroidGo, randomSizeToOreNodes.MaxOreNodes);
            return asteroidGo;
        }

        private static void PlaceRandomOreNodesInsideAsteroid(GameObject asteroidGo, int maxOreNodes)
        {
            var oreNodesGo = new List<GameObject>();
            for (var i = 0; i < maxOreNodes; i++)
            {
                var oreNodeGo = GameObjectFactory.OreNode(asteroidGo.transform);
                PlaceOreNodeInsideAsteroid(asteroidGo, oreNodeGo, oreNodesGo);
                oreNodesGo.Add(oreNodeGo);
            }
        }

        private static void PlaceOreNodeInsideAsteroid(GameObject asteroid, 
            GameObject oreNode, List<GameObject> oreNodesGo)
        {
            var polygon = asteroid.GetComponent<PolygonCollider2D>();
            var astBounds = polygon.bounds;
            var isOreNodeInside = false;
            var oreNodeSr = oreNode.GetComponent<SpriteRenderer>();
            var oreNodeBounds = new Bounds(oreNodeSr.bounds.center, oreNodeSr.bounds.size);
            var rndPoint = Vector3.zero;
            
            do
            {
                rndPoint = new Vector2(Random.Range(astBounds.min.x, astBounds.max.x),
                    Random.Range(astBounds.min.y, astBounds.max.y));
                oreNodeBounds.center = rndPoint;
                var corners = MathHelper.GetSquareCorners(oreNodeBounds);
                isOreNodeInside = MathHelper.PointsInsidePolygon(corners, polygon);

                //If center and corners areinside asteroid check if it doesn't overlap with other ore nodes
                if(!isOreNodeInside) continue;
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
