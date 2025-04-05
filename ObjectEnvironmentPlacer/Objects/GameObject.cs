namespace ObjectEnvironmentPlacer.Objects
{
    public class GameObject2D
    {
        public Guid ID { get; set; }
        public Guid EnvironmentID { get; set; }
        public int PrefabID { get; set; }
        public float PositionX { get; set; }
        public float PositionY { get; set; }
        public float ScaleX { get; set; }
        public float ScaleY { get; set; }
        public float RotationZ { get; set; }
        public int SortingLayer { get; set; }

    }
}
