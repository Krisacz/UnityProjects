namespace Assets.Scripts.Models
{
    //Node Density - chance for max amount
    public class NDensity
    {
        public float Chance { get; set; }
        public int Amuont { get; set; }

        public NDensity(float chance, int amuont)
        {
            Chance = chance;
            Amuont = amuont;
        }
    }
}