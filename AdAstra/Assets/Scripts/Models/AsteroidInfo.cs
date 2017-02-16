using System.Collections.Generic;

namespace Assets.Scripts.Models
{
    public class AsteroidInfo
    {
        public List<int> SpriteIds { get; set; }
        public AsteroidSize AsteroidSize { get; set; }
        public List<AsteroidSizeToOreNodes> SizeToOreNodes { get; set; }
    }
}