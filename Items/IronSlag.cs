namespace Core
{
    public class IronSiliconSlag : Item
    {
        public override ItemType Type => ItemType.IronSiliconSlag;
        public override string Name => "Iron silicon slag";
        public override string? ChemicalFormula => "FeS₂";
        public IronSiliconSlag(uint quantity) : base(quantity) { }
    }
}