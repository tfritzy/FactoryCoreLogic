namespace Core
{
    public class ActiveItems : Inventory
    {
        public override ComponentType Type => ComponentType.ActiveItems;

        public ActiveItems(Entity owner, Item?[] items, int width, int height) : base(owner, items, width, height)
        {
        }

        public ActiveItems(Entity owner, int width, int height) : base(owner, width, height)
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