namespace Core
{
    public class LocationUpdate : Update
    {
        public override UpdateType Type => UpdateType.Location;
        public Point2Int Location { get; private set; }

        public LocationUpdate(int x, int y)
        {
            Location = new Point2Int(x, y);
        }

        public LocationUpdate(Point2Int location)
        {
            Location = location;
        }
    }
}