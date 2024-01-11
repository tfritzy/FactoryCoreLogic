namespace Core
{
    public class BuildingRemoved : Update
    {
        public override UpdateType Type => UpdateType.BuildingRemoved;
        public ulong Id { get; private set; }
        public Point3Int GridPosition { get; private set; }

        public BuildingRemoved(ulong id, Point3Int gridPosition)
        {
            Id = id;
            GridPosition = gridPosition;
        }
    }
}