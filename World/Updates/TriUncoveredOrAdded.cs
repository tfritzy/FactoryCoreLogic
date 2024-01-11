namespace Core
{
    public class TriUncoveredOrAdded : Update
    {
        public override UpdateType Type => UpdateType.TriUncoveredOrAdded;
        public Point3Int GridPosition { get; private set; }
        public HexSide Side { get; private set; }

        public TriUncoveredOrAdded(Point3Int location, HexSide side)
        {
            GridPosition = location;
            Side = side;
        }
    }
}