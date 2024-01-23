namespace Core
{
    public class FuelInventory : Inventory
    {
        public override ComponentType Type => ComponentType.FuelInventory;

        public FuelInventory(Schema.FuelInventory schema, Entity owner)
            : base(schema.Inventory, owner)
        {
        }

        public FuelInventory(Entity owner, int width, int height) : base(owner, width, height)
        {
        }

        public override bool CanAddItem(ItemType itemType, ulong quantity)
        {
            if (Item.ItemProperties[itemType].Combustion == null)
            {
                return false;
            }

            return base.CanAddItem(itemType, quantity);
        }

        public override bool AddItem(Item item)
        {
            if (item.Combustion == null)
            {
                return false;
            }

            return base.AddItem(item);
        }

        public override bool AddItem(Item item, int index)
        {
            if (item.Combustion == null)
            {
                return false;
            }

            return base.AddItem(item, index);
        }

        public override Schema.OneofComponent ToSchema()
        {
            return new Schema.OneofComponent
            {
                FuelInventory = new Schema.FuelInventory
                {
                    Inventory = base.ToSchema().Inventory,
                },
            };
        }
    }
}