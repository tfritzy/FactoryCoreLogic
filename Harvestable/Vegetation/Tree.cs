namespace FactoryCore
{
    public class Tree : Harvestable
    {
        public override ItemType ProducedItemType => ItemType.Wood;
        public override HarvestableType HarvestableType => HarvestableType.Tree;
        public override int MaxHarvestItems => 16;
        public override Item BuildHarvestedItem()
        {
            return new Wood();
        }
    }
}
