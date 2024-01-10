namespace Core
{
    public class WornItems : Inventory
    {
        public override ComponentType Type => ComponentType.WornItems;

        public WornItems(Entity owner, Item?[] items, int width, int height) : base(owner, items, width, height)
        {
        }

        public WornItems(Entity owner, int width, int height) : base(owner, width, height)
        {
        }

        public override Schema.OneofComponent ToSchema()
        {
            var schema = new Schema.OneofComponent
            {
                ActiveItems = new Schema.ActiveItems()
                {
                    Inventory = base.ToSchema().Inventory,
                }
            };
            return schema;
        }
    }
}