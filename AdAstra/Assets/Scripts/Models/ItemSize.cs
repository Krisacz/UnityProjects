namespace Assets.Scripts.Models
{
    public class ItemSize
    {
        public int Width { get; set; }
        public int Height { get; set; }

        public ItemSize(int width, int height)
        {
            Width = width;
            Height = height;
        }
    }
}