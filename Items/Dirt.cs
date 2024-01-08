namespace Core
{
    public class Dirt : Item
    {
        public override ItemType Type => ItemType.Dirt;
        public override ulong MaxStack => 4;
        public Dirt(ulong quantity) : base(quantity) { }
        private const string name = "Dirt";
        public override string Name => name;
        public override string? ChemicalFormula => null;
    }
}