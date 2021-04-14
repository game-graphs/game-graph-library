namespace GDT.Utility.Visualization.DataStructures
{
    public struct WorldData
    {
        public TileType TileType;

        public WorldData(TileType tileType)
        {
            TileType = tileType;
        }
    }

    public enum TileType
    {
        City, Road, Forrest, Cave, WaterWay, Island
    }
}