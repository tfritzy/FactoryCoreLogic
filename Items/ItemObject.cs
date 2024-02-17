namespace Core
{
    // An object living in the world as an object. rolls around with physics
    // and can be picked up.
    public class ItemObject
    {
        public Item Item;
        public Point3Float Position;
        public Point3Float Velocity { get; set; }

        public ItemObject(Item item, Point3Float position, Point3Float velocity)
        {
            this.Item = item;
            this.Position = position;
            this.Velocity = velocity;
        }

        public Schema.ItemObject ToSchema()
        {
            return new Schema.ItemObject()
            {
                Item = Item.ToSchema(),
                Position = Position.ToSchema(),
                Velocity = Velocity.ToSchema(),
            };
        }

        public static ItemObject FromSchema(Schema.ItemObject schema)
        {
            return new ItemObject(
                Item.FromSchema(schema.Item),
                Point3Float.FromSchema(schema.Position),
                Point3Float.FromSchema(schema.Velocity)
            );
        }

        public void ClientTick(float deltaTime)
        {
            Position += Velocity * deltaTime;
        }
    }
}