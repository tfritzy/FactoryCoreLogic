namespace Core
{
    public class LigniteCoal : Item
    {
        public override ItemType Type => ItemType.LigniteCoal;
        public override int MaxStack => 16;
        public LigniteCoal(int quantity) : base(quantity) { }
        override public float Width => .3f;
        private const string name = "Lignite coal";
        public override string Name => name;
        public override string? ChemicalFormula => null;
        public override CombustionProperties? Combustion => properties;
        private static CombustionProperties properties = new()
        {
            BurnRateKgPerSecond = 1f,
            CalorificValue_JoulesPerKg = 12_000_000
        };
    }
}