namespace Core
{
    // An object living in the world as an object. rolls around with physics
    // and can be picked up.
    public class ItemObject
    {
        public Item Item;
        public Point3Float Position;

        public ItemObject(Item item, Point3Float position)
        {
            this.Item = item;
            this.Position = position;
        }
    }
}