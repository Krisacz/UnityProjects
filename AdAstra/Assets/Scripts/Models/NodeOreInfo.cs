using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Controllers;

namespace Assets.Scripts.Models
{
    public class NodeOreInfo
    {
        public float Rarity { get; set; }
        public ItemId  ItemId  { get; set; }
        public int ScanLevelRequired { get; set; }
        public List<NodeDensity> Density { get; set; }

        public NodeOreInfo(float rarity, ItemId itemId, int scanLevelRequired, params NodeDensity[] density)
        {
            Rarity = rarity;
            ItemId = itemId;
            ScanLevelRequired = scanLevelRequired;
            Density = density.ToList();
        }
    }
}