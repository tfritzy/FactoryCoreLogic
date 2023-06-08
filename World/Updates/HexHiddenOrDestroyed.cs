namespace Core
{
    // Hidden can mean destroyed in this context. They're the same
    // to the client's perspective.
    public class HexHiddenOrDestroyed : Update
    {
        public override UpdateType Type => UpdateType.HexHiddenOrDestroyed;
        public Point3Int GridPosition { get; private set; }

        public HexHiddenOrDestroyed(int x, int y, int z)
        {
            GridPosition = new Point3Int(x, y, z);
        }

        public HexHiddenOrDestroyed(Point3Int location)
        {
            GridPosition = location;
        }
    }
}