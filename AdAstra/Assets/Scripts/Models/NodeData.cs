namespace Assets.Scripts.Models
{
    public class NodeData
    {
        public ItemId ItemId { get; set; }
        public int Max { get; set; }
        public int Count { get; set; }
        public int ScanLevelRequired { get; set; }

        public NodeData(ItemId itemId, int max, int scanLevelRequired)
        {
            ItemId = itemId;
            Max = max;
            Count = max;
            ScanLevelRequired = scanLevelRequired;
        }
    }
}