using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Db;
using Assets.Scripts.Models;

namespace Assets.Scripts.Controllers
{
    public class NodeGenerator
    {
        private static List<NodeOreInfo>  _nodeOreInfo = new List<NodeOreInfo>();

        public static void Init()
        {
            var allOre = ItemsDatabase.GetAllWithFunctionProperty(FunctionProperty.OreRarity);
            foreach (var itemId in allOre)
            {
                var item = ItemsDatabase.Get(itemId);

                //We expect to have exactly 3 int numbers in density property each split by "|" pipe
                var densityStr = item.FunctionProperties.AsString(FunctionProperty.OreDensity).Split('|');
                
                var nodeOreInfo = new NodeOreInfo(
                    item.FunctionProperties.AsFloat(FunctionProperty.OreRarity),
                    itemId,
                    item.FunctionProperties.AsInt(FunctionProperty.ScanLevelRequired),
                    new NodeDensity(10f, int.Parse(densityStr[0])),
                    new NodeDensity( 5f, int.Parse(densityStr[1])),
                    new NodeDensity( 1f, int.Parse(densityStr[2]))
                    );
                _nodeOreInfo.Add(nodeOreInfo);
            }
        }

        public static NodeData New()
        {
            var nodeChances = _nodeOreInfo.Select(ni => ni.Rarity).ToArray();
            var nodeIndex = MathHelper.RandomRange(nodeChances);
            var nodeInfo = _nodeOreInfo[nodeIndex];
            
            var densityChances = nodeInfo.Density.Select(d => d.Chance).ToArray();
            var densityIndex = MathHelper.RandomRange(densityChances);
            var density = nodeInfo.Density[densityIndex];
            
            return new NodeData(nodeInfo.ItemId, density.Amuont, nodeInfo.ScanLevelRequired);
        } 
    }
}