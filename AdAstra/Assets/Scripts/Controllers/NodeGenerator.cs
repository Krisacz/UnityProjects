using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Models;

namespace Assets.Scripts.Controllers
{
    public class NodeGenerator
    {
        private static readonly List<NodeInfo> NodesData = new List<NodeInfo>()
        {
            new NodeInfo(20f, ItemId.Ice,          1, new NDensity(10f, 5), new NDensity(5f, 10), new NDensity(1f, 20)),
            new NodeInfo(15f, ItemId.PlasteelOre,  1, new NDensity(10f, 3), new NDensity(5f,  5), new NDensity(1f,  7)),
            new NodeInfo(10f, ItemId.IronOre,      1, new NDensity(10f, 3), new NDensity(5f,  5), new NDensity(1f,  7)),
            new NodeInfo( 7f, ItemId.CopperOre,    2, new NDensity(10f, 3), new NDensity(5f,  5), new NDensity(1f,  7)),
            new NodeInfo( 7f, ItemId.TinOre,       2, new NDensity(10f, 3), new NDensity(5f,  5), new NDensity(1f,  7)),
            new NodeInfo( 5f, ItemId.GoldOre,      3, new NDensity(10f, 2), new NDensity(5f,  3), new NDensity(1f,  4)),
            new NodeInfo( 5f, ItemId.PlatiniumOre, 3, new NDensity(10f, 2), new NDensity(5f,  3), new NDensity(1f,  4)),
            new NodeInfo( 3f, ItemId.TitaniumOre,  4, new NDensity(10f, 2), new NDensity(5f,  3), new NDensity(1f,  4)),
            new NodeInfo( 2f, ItemId.CrystalOre,   4, new NDensity(10f, 1), new NDensity(5f,  2), new NDensity(1f,  3)),
            new NodeInfo( 1f, ItemId.DiamondOre,   5, new NDensity(10f, 1), new NDensity(5f,  2), new NDensity(1f,  3)),
        };

        public static NodeData New()
        {
            var nodeChances = NodesData.Select(ni => ni.Chance).ToArray();
            var nodeIndex = MathHelper.RandomRange(nodeChances);
            var nodeInfo = NodesData[nodeIndex];
            
            var densityChances = nodeInfo.Density.Select(d => d.Chance).ToArray();
            var densityIndex = MathHelper.RandomRange(densityChances);
            var density = nodeInfo.Density[densityIndex];
            
            return new NodeData(nodeInfo.ItemId, density.Amuont, nodeInfo.ScanLevelRequired);
        } 
    }
}