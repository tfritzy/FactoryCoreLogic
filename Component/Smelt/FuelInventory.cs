namespace Core
{
    public class FuelInventory : Inventory
    {
        public FuelInventory(Entity owner, int width, int height) : base(owner, width, height)
        {
        }

        public override bool CanAddItem(ItemType itemType, uint quantity)
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
    }
}