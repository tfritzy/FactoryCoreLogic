namespace Core
{
    public class BuildingAdded : Update
    {
        public override UpdateType Type => UpdateType.BuildingAdded;
        public Point2Int GridPosition { get; private set; }

        public BuildingAdded(int x, int y)
        {
            GridPosition = new Point2Int(x, y);
        }

        public BuildingAdded(Point2Int location)
        {
            GridPosition = location;
        }
    }
}