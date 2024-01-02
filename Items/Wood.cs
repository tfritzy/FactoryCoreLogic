namespace Core
{
    public class Wood : Item
    {
        public override ItemType Type => ItemType.Wood;
        public override uint MaxStack => 4;
        public Wood(ulong quantity) : base(quantity) { }
        private const string name = "Wood";
        public override string Name => name;
        public override string? ChemicalFormula => "C₆H₁₀O₅";
    }
}