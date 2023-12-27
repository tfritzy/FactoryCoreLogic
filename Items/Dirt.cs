namespace Core
{
    public class Dirt : Item
    {
        public override ItemType Type => ItemType.Dirt;
        public override uint MaxStack => 4;
        public Dirt(uint quantity) : base(quantity) { }
        private const string name = "Dirt";
        public override string Name => name;
        public override string? ChemicalFormula => null;
    }
}