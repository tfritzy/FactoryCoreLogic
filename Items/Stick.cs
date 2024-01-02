namespace Core
{
    public class Stick : Item
    {
        public override ItemType Type => ItemType.Stick;
        public Stick(ulong quantity) : base(quantity) { }
        public override float Width => .3f;
        private const string name = "Stick";
        public override string Name => name;
        public override string? ChemicalFormula => "C₆H₁₀O₅";
        public override uint MaxStack => 64;
    }
}