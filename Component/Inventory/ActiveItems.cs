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

        public override Schema.Component ToSchema()
        {
            Schema.Inventory? inventory = base.ToSchema() as Schema.Inventory;

            if (inventory == null)
                throw new System.Exception("Parent's toSchema was unexpectedly not an InventoryComponent");

            return new Schema.ActiveItems
            {
                Items = inventory.Items,
                Width = inventory.Width,
                Height = inventory.Height,
            };
        }
    }
}