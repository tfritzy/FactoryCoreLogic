namespace Core
{
    public interface WorldApi
    {
        public void SetUnitLocation(ulong unitId, Point3Float location);
        public void PluckBush(ulong unitId, Point2Int pos);
        public void SetItemObjectPos(ulong itemId, Point3Float pos, Point3Float rotation);
        public void PickupItem(ulong pickerUperId, ulong itemId);
    }
}