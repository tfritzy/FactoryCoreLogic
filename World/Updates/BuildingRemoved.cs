namespace Core
{
    public class BuildingRemoved : Update
    {
        public override UpdateType Type => UpdateType.BuildingRemoved;
        public Point2Int GridPosition { get; private set; }

        public BuildingRemoved(int x, int y)
        {
            GridPosition = new Point2Int(x, y);
        }

        public BuildingRemoved(Point2Int location)
        {
            GridPosition = location;
        }
    }
}