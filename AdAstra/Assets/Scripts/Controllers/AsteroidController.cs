using System.Collections.Generic;
using UnityEngine;

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

    public class AsteroidController
    {
        private List<AsteroidInfo> _infos = new List<AsteroidInfo>()
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

        private void PlaceOreNodeInsideAsteroid(GameObject asteroid, GameObject oreNode)
        {
            var polygon = asteroid.GetComponent<PolygonCollider2D>();
            var bounds = polygon.bounds;
            var isOreNodeInside = false;
            do
            {
                var rndPoint = new Vector2(Random.Range(bounds.min.x, bounds.max.x),
                                            Random.Range(bounds.min.y, bounds.max.y));
                oreNode.transform.position = rndPoint;
                var corners = MathHelper.GetSquareCorners(oreNode.GetComponent<SpriteRenderer>().bounds);
                isOreNodeInside = MathHelper.PointsInsidePolygon(corners, polygon);
            }
            while (!isOreNodeInside);
        }

        private void PlaceOreNodesInsideAsteroid(GameObject asteroid, int oreNodesCount)
        {
            
        }
    }
}
