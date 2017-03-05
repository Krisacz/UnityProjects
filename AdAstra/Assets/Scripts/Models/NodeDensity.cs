namespace Assets.Scripts.Models
{
    //Node Density - chance for max amount
    public class NodeDensity
    {
        public float Chance { get; set; }
        public int Amuont { get; set; }

        public NodeDensity(float chance, int amuont)
        {
            Chance = chance;
            Amuont = amuont;
        }
    }
}