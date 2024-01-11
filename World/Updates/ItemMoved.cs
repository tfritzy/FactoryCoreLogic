namespace Core
{
    public class ItemMoved : Update
    {
        public override UpdateType Type => UpdateType.ItemMoved;
        public ulong Id;
        public Point3Float Pos;
        public Point3Float Rotation;

        public ItemMoved(ulong id, Point3Float pos, Point3Float rotation)
        {
            this.Pos = pos;
            this.Id = id;
            this.Rotation = rotation;
        }
    }
}