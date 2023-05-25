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

        public override Schema.Component ToSchema()
        {
            Schema.Inventory? inventory = base.ToSchema() as Schema.Inventory;

            if (inventory == null)
                throw new System.Exception("Parent's toSchema was unexpectedly not an InventoryComponent");

            return new Schema.WornItems
            {
                Items = inventory.Items,
                Width = inventory.Width,
                Height = inventory.Height,
            };
        }
    }
}