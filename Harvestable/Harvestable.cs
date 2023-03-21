namespace FactoryCore
{
    public abstract class Harvestable
    {
        public abstract ItemType ProducedItemType { get; }
        public abstract HarvestableType HarvestableType { get; }
        public abstract Item BuildHarvestedItem();
        public abstract int MaxHarvestItems { get; }
        public int RemainingItems { get; private set; }

        public Harvestable()
        {
            RemainingItems = MaxHarvestItems;
        }

        public Item? Harvest()
        {
            if (RemainingItems > 0)
            {
                RemainingItems--;
                return BuildHarvestedItem();
            }
            else
            {
                return null;
            }
        }
    }
}