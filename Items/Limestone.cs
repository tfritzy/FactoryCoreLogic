namespace Core
{
    public class Limestone : Item
    {
        public override ItemType Type => ItemType.Limestone;
        public override int MaxStack => 8;
        public Limestone(int quantity) : base(quantity) { }
        public Limestone() : base() { }
        public override float Width => .4f;
        private const string name = "Limestone";
        public override string Name => name;
        public override string? ChemicalFormula => "CaCO₃";
    }
}