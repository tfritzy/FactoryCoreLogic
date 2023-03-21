namespace FactoryCore
{
    public class StoneHex : Hex
    {
        public override ItemType ProducedItemType => ItemType.Stone;
        public override HarvestableType HarvestableType => HarvestableType.StoneHex;
        public override HexType Type => HexType.Stone;
        public override int MaxHarvestItems => 8;
        public override Item BuildHarvestedItem()
        {
            return new Stone();
        }
    }
}