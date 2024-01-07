namespace Core
{
    public interface WorldApi
    {
        public void SetUnitLocation(ulong unitId, Point3Float location);
        public void SetItemObjectPos(ulong itemId, Point3Float pos, Point3Float rotation);
    }
}