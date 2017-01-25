namespace Assets.Scripts.Models
{
    public class StructureSize
    {
        public int Width { get; set; }
        public int Height { get; set; }

        public StructureSize(int width, int height)
        {
            Width = width;
            Height = height;
        }
    }
}