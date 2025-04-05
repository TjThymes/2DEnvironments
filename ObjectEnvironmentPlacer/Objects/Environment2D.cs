using System.Globalization;

namespace ObjectEnvironmentPlacer.Objects
{
    public class Environment2D
    {
        public Guid ID { get; set; }
        //public Guid UserId { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        //public int Width = 1920;
        //public int Height = 1080;

        public List<GameObject2D> Objects { get; set; } = new List<GameObject2D>();
    }
}