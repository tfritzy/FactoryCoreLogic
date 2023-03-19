
namespace FactoryCore
{
    public class DirtHex : Hex
    {
        public override HarvestableType HarvestableType => HarvestableType.DirtHex;
        public override HexType Type => HexType.Dirt;
        public override int MaxHarvestItems => 8;
        public override Item BuildHarvestedItem()
        {
            return new Dirt();
        }
    }
}