namespace FactoryCore
{
    public class Harvestable : Cell
    {
        public override CellType Type => CellType.Harvestable;
        public ItemType ProducedItemType { get; }
        public int MaxHarvestItems { get; }
        public int RemainingItems { get; private set; }
        public HarvestableType HarvestableType { get; private set; }

        public Harvestable(EntityComponent owner, ItemType produces, int maxItems, HarvestableType harvestableType) : base(owner)
        {
            this.ProducedItemType = produces;
            this.MaxHarvestItems = maxItems;
            this.RemainingItems = maxItems;
            this.HarvestableType = harvestableType;
        }

        public Item BuildHarvestedItem() => Item.Create(ProducedItemType);

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