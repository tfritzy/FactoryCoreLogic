namespace Core
{
    public class IronSiliconSlag : Item
    {
        public override ItemType Type => ItemType.IronSiliconSlag;
        public override string Name => "Iron silicon slag";
        public override string? ChemicalFormula => "FeS₂";
        public override ulong MaxStack => 200_000_000;
        public override UnitType Units => UnitType.Milligram;
        public IronSiliconSlag(ulong quantity) : base(quantity) { }
    }
}