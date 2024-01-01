namespace Core
{
    public interface WorldApi
    {
        public void SetUnitLocation(ulong unitId, Point3Float location);
        public void PluckBush(ulong unitId, Point2Int pos);
    }
}