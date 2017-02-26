using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Controllers;

namespace Assets.Scripts.Models
{
    public class NodeInfo
    {
        public float Chance { get; set; }
        public ItemId  ItemId  { get; set; }
        public int ScanLevelRequired { get; set; }
        public List<NDensity> Density { get; set; }

        public NodeInfo(float chance, ItemId itemId, int scanLevelRequired, params NDensity[] density)
        {
            Chance = chance;
            ItemId = itemId;
            ScanLevelRequired = scanLevelRequired;
            Density = density.ToList();
        }
    }
}