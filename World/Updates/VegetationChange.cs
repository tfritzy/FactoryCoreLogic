namespace Core
{
    public class VegetationChange : Update
    {
        public override UpdateType Type => UpdateType.VegetationChange;
        public Point2Int GridPosition { get; private set; }
        public VegetationType NewVegeType { get; private set; }

        public VegetationChange(Point2Int location, VegetationType newType)
        {
            GridPosition = location;
            NewVegeType = newType;
        }
    }
}