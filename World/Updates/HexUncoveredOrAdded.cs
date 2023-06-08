namespace Core
{
    public class HexUncoveredOrAdded : Update
    {
        public override UpdateType Type => UpdateType.HexUncoveredOrAdded;
        public Point3Int GridPosition { get; private set; }

        public HexUncoveredOrAdded(int x, int y, int z)
        {
            GridPosition = new Point3Int(x, y, z);
        }

        public HexUncoveredOrAdded(Point3Int location)
        {
            GridPosition = location;
        }
    }
}