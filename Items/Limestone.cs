namespace Core
{
    public class Limestone : Item
    {
        public override ItemType Type => ItemType.Limestone;
        public Limestone(ulong quantity) : base(quantity) { }
        public override float Width => .4f;
        private const string name = "Limestone";
        public override string Name => name;
        public override string? ChemicalFormula => "CaCO₃";
        public override uint MaxStack => 200_000_000;
        public override UnitType Units => UnitType.Milligram;
    }
}