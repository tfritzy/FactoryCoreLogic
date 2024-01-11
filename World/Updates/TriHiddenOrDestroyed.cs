namespace Core
{
    // Hidden can mean destroyed in this context. They're the same
    // to the client's perspective.
    public class TriHiddenOrDestroyed : Update
    {
        public override UpdateType Type => UpdateType.TriHiddenOrDestroyed;
        public Point3Int GridPosition { get; private set; }
        public HexSide Side { get; private set; }

        public TriHiddenOrDestroyed(Point3Int location, HexSide side)
        {
            GridPosition = location;
            Side = side;
        }
    }
}