namespace Core
{
    public class WornItemsComponent : InventoryComponent
    {
        public override ComponentType Type => ComponentType.WornItems;

        public WornItemsComponent(Entity owner, Item?[] items, int width, int height) : base(owner, items, width, height)
        {
        }

        public WornItemsComponent(Entity owner, int width, int height) : base(owner, width, height)
        {
        }

        public override Schema.Component ToSchema()
        {
            Schema.InventoryComponent? inventory = base.ToSchema() as Schema.InventoryComponent;

            if (inventory == null)
                throw new System.Exception("Parent's toSchema was unexpectedly not an InventoryComponent");

            return new Schema.WornItemsComponent
            {
                Items = inventory.Items,
                Width = inventory.Width,
                Height = inventory.Height,
            };
        }
    }
}