namespace Core
{
    public class Leaves : Item
    {
        public override ItemType Type => ItemType.Leaves;
        public Leaves(ulong quantity) : base(quantity) { }
        public override float Width => .3f;
        private const string name = "Leaves";
        public override string Name => name;
        public override string? ChemicalFormula => null;
        public override uint MaxStack => 64;
    }
}