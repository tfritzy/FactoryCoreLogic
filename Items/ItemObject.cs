using Schema;

namespace Core
{
    // An object living in the world as an object. rolls around with physics
    // and can be picked up.
    public class ItemObject
    {
        public Item Item;
        public Point3Float Position;
        public Point3Float Rotation;

        public ItemObject(Item item, Point3Float position, Point3Float rotation)
        {
            this.Item = item;
            this.Position = position;
            this.Rotation = rotation;
        }

        public Schema.ItemObject ToSchema()
        {
            return new Schema.ItemObject()
            {
                Item = Item.ToSchema(),
                Position = Position.ToSchema(),
                Rotation = Rotation.ToSchema(),
            };
        }

        public static ItemObject FromSchema(Schema.ItemObject schema)
        {
            return new ItemObject(
                Item.FromSchema(schema.Item),
                Point3Float.FromSchema(schema.Position),
                Point3Float.FromSchema(schema.Rotation)
            );
        }
    }
}