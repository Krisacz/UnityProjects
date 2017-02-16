using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Models;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.Scripts.Controllers
{
    public class AsteroidsController : MonoBehaviour
    {
        public FlightController FlightController;
        
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
        
        #region TESTING
        private int min = 1;
        private int max = 1;
        private int repeat = 1;

        private float _angle = 0f;
        private float _oldAngle = 0f;
        private float _distance = 0f;
        private float _oldDistance = 0f;

        void OnGUI()
        {
            return;

            //Min slider
            GUI.Label(new Rect(5, 255, 250, 20), "Angle: [" + _angle + "]");
            _angle = (int)GUI.HorizontalSlider(new Rect(5, 275, 360, 30), _angle, 0.0f, 359.0f);

            GUI.Label(new Rect(5, 330, 250, 20), "Distance: [" + _distance + "]");
            _distance = (int)GUI.HorizontalSlider(new Rect(5, 350, 360, 30), _distance, 0.0f, 100f);

            if (Math.Abs(_angle - _oldAngle) > 0.01f || Math.Abs(_distance - _oldDistance) > 0.01f)
            {
                var v = MathHelper.Angle2Vector(_angle);
                var vDis = v * _distance;
                GameObject.Find("X").transform.position = vDis;

                var a1 = MathHelper.Angle2Vector(_angle + 90f);
                GameObject.Find("X1").transform.position = vDis + a1;

                var a2 = MathHelper.Angle2Vector(_angle - 90f);
                GameObject.Find("X2").transform.position = vDis + a2;

                _oldAngle = _angle;
                _oldDistance = _distance;
            }


            if (GUI.Button(new Rect(10, 200, 100, 50), "Spawn"))
            {
                foreach (var gameObj in FindObjectsOfType<GameObject>())
                {
                    if (gameObj.name.Equals("Asteroid")) Destroy(gameObj);
                }

                SpawnAsteroids();

                //var v = MathHelper.Angle2Vector(_angle);
                //GameObject.Find("X").transform.position = v;
            }

            return;

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

                    //TODO If you need to use it make an overload
                    //SpawnAsteroids(min, max);
                }
            }

            if (GUI.Button(new Rect(10, 400, 100, 50), "XXX"))
            {
                var m = -1f;
                for (var i = 0; i < 10000; i++)
                {
                    var r = Random.Range(0f, 1f);
                    m = Mathf.Max(m, r);
                }
                Log.Info("Max: " + m);
            }
        }

        #endregion

        #region SPAWN ASTEROIDS
        public void SpawnAsteroids()
        {
            //Chances for number of nodes during this spawn
            var chances = new[] {10f, 5f, 2f, 1f, 0.5f, 0.1f};

            //Number of nodes
            var nodesCh = new[] {6, 8, 10, 12, 16, 20};

            //Randomized number of nodes
            var rnd = MathHelper.RandomRange(chances);
            var nodes = nodesCh[rnd];
            var log = "Nodes Total: " + nodes;

            //Split nodes into separate asteroids
            var astNod = new List<int>();
            while (nodes > 0)
            {
                var r = Mathf.Min(nodes, Random.Range(4, 8));
                astNod.Add(r);
                nodes -= r;
            }
            log += " # Split into " + astNod.Count + " asteroids";

            var asteroids = new List<Asteroid>();

            //Get all asteroids with nodes
            foreach (var count in astNod)
            {
                var asteroidInfo = GetRandomAsteroidInfoFromOreNodeCount(count);
                var asteroid = GetRandomAsteroidFromAsteroidsInfo(asteroidInfo);
                asteroids.Add(asteroid);
            }

            //And those without nodes
            var emptyAsteroids = Random.Range(astNod.Count, astNod.Count*2 + 1);
            for (var i = 1; i <= emptyAsteroids; i++)
            {
                var asteroidInfo = GetRandomAsteroidInfoFromOreNodeCount(0);
                var asteroid = GetRandomAsteroidFromAsteroidsInfo(asteroidInfo);
                asteroids.Add(asteroid);
            }
            log += " # Additional " + emptyAsteroids + " empty asteroids.";

            //World Placement
            var baseAngle = FlightController.DirectionAngle;
            var modifiedAngle = baseAngle + Random.Range(-45f, 45f);
            var baseVector = MathHelper.Angle2Vector(modifiedAngle);
            //35f for particles emmiter distance + 15f spawner circle radious
            var actualDistance = baseVector*(35f + 15f);
            this.transform.position = actualDistance;
            var radious = this.transform.GetComponent<SpriteRenderer>().bounds.extents.x;
            var rotationChance = new float[] {5f, 3f, 1f, 3f, 1f};
            var rotation = new float[] {15f, 20f, 25f, 10f, 5f};
            var asteroidsCollection = GameObject.Find("AsteroidsCollection").transform;

            //TODO Consider also checking if spwaned asteroid is not overlapping existing asteroids
            for (var i = 0; i < asteroids.Count; i++)
            {
                var rndPoint = MathHelper.RandomInCircle(this.transform.position, radious);
                var asteroid = asteroids[i];
                asteroid.GameObject.transform.position = rndPoint;
                var component = asteroid.GameObject.GetComponent<Rigidbody2D>();
                var rotationSpeed = rotation[MathHelper.RandomRange(rotationChance)];
                var rotationDir = MathHelper.RandomBool() ? 1 : -1;
                component.angularVelocity = rotationSpeed*rotationDir;
                asteroid.GameObject.transform.SetParent(asteroidsCollection);
                asteroid.InitialDistance = (int) Vector2.Distance(Vector2.zero,
                    asteroid.GameObject.transform.position);
            }

            //Sort by distance
            asteroids.Sort();

            //"Start" them in order
            var delay = 1.0f;
            const float speed = 10f;
            foreach (var asteroid in asteroids)
            {
                var speedMod = SpeedModFromSize(asteroid.AsteroidSize, asteroid.Scale);
                var rigitbody = asteroid.GameObject.GetComponent<Rigidbody2D>();
                SequenceController.Instance.AddSingleTimeLink(
                    delay,
                    () =>
                    {
                        rigitbody.velocity = new Vector2(
                            Mathf.Lerp(0, speed*speedMod*baseVector.x*-1, 0.8f),
                            Mathf.Lerp(0, speed*speedMod*baseVector.y*-1, 0.8f));
                    });
                delay += 1.0f;
            }


            Log.Info(log);

            //Chain it infinitely with 20 sec delay between
            SequenceController.Instance.AddSingleTimeLink(20f, SpawnAsteroids);
        }

        #endregion

        #region SPEED MOD FROM SIZE
        private static float SpeedModFromSize(AsteroidSize asteroidSize, float scale)
        {
            switch (asteroidSize)
            {
                case AsteroidSize.Tiny:
                    return 0.650f/scale;
                case AsteroidSize.Small:
                    return 0.550f/scale;
                case AsteroidSize.Medium:
                    return 0.600f/scale;
                case AsteroidSize.Large:
                    return 0.500f/scale;
                default:
                    throw new ArgumentOutOfRangeException("asteroidSize", asteroidSize, null);
            }
        }
        #endregion

        #region GET RANDOM ASTEROID INFO FROM ORE NODE COUNT
        private AsteroidInfo GetRandomAsteroidInfoFromOreNodeCount(int count)
        {
            if (count < 0 || count > 8)
            {
                Log.Error("AsteroidsController", "GetRandomAsteroidInfoFromOreNodeCount",
                    "Count needs to be between 0 and 8, provided: " + count);
                return null;
            }

            var available = GetAsteroidsInfoForOreNodeCount(count);

            //In case we've somehow got no results
            if (available.Count == 0)
                Log.Error("AsteroidsController", "GetRandomAsteroidInfoFromOreNodeCount",
                    "No results?!?! Count: " + count);

            //Randomize index from available asteroids
            var rndIndex = Random.Range(0, available.Count);

            //Return
            return available[rndIndex];
        }
        #endregion

        #region GET ASTEROIDS INFO FOR ORE NODE COUNT
        private List<AsteroidInfo> GetAsteroidsInfoForOreNodeCount(int count)
        {
            if (count < 0 || count > 8)
                Log.Error("AsteroidsController", "GetAsteroidsInfoForOreNodeCount",
                    "Count needs to be between 0 and 8, provided: " + count);

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

                foreach (var a in asteroidInfo.SizeToOreNodes.Where(a => a.MaxOreNodes == count))
                {
                    ai.SizeToOreNodes.Add(a);
                }

                if (ai.SizeToOreNodes.Count > 0) list.Add(ai);
            }

            return list;
        }
        #endregion

        #region GET RANDOM ASTEROID FROM ASTEROIDS INFO
        private Asteroid GetRandomAsteroidFromAsteroidsInfo(AsteroidInfo ai)
        {
            var randomSpriteId = ai.SpriteIds[Random.Range(0, ai.SpriteIds.Count)];
            var randomSizeToOreNodes = ai.SizeToOreNodes[Random.Range(0, ai.SizeToOreNodes.Count)];
            var randomScale = Random.Range(randomSizeToOreNodes.MinScale, randomSizeToOreNodes.MaxScale);
            var asteroidGo = GameObjectFactory.Asteroid(randomSpriteId, randomScale, this.transform);
            var asteroid = new Asteroid()
            {
                SpriteId = randomSpriteId,
                Scale = randomScale,
                GameObject = asteroidGo,
                NodesCount = randomSizeToOreNodes.MaxOreNodes,
                AsteroidSize = ai.AsteroidSize
            };

            PlaceRandomOreNodesInsideAsteroid(asteroidGo, randomSizeToOreNodes.MaxOreNodes);
            return asteroid;
        }
        #endregion

        #region PLACE RANDOM ORE NODES INSIDE ASTEROID
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
        #endregion

        #region PLACE ORE NODE INSIDE ASTEROID
        private void PlaceOreNodeInsideAsteroid(GameObject asteroid, GameObject oreNode, List<GameObject> oreNodesGo)
        {
            var polygon = asteroid.GetComponent<PolygonCollider2D>();
            var astBounds = polygon.bounds;
            var isOreNodeInside = false;
            var oreNodeSr = oreNode.GetComponent<SpriteRenderer>();
            var oreNodeBounds = new Bounds(oreNodeSr.bounds.center, oreNodeSr.bounds.size);
            var rndPoint = Vector3.zero;
            var emergencyBreak = 10000;
            do
            {
                emergencyBreak--;
                if (emergencyBreak <= 0) break;

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
            } while (!isOreNodeInside);

            oreNodeSr.transform.position = rndPoint;
        }
        #endregion
    }
}
