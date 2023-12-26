namespace Core
{
    public class Dirt : Item
    {
        public override ItemType Type => ItemType.Dirt;
        public override int MaxStack => 4;
        public Dirt(int quantity) : base(quantity) { }
        public Dirt() : base() { }
        private const string name = "Dirt";
        public override string Name => name;
        public override string? ChemicalFormula => null;
    }
}