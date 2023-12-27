namespace Core
{
    public class AnthraciteCoal : Item
    {
        public override ItemType Type => ItemType.AnthraciteCoal;
        public override uint MaxStack => 50_000_000;
        public override UnitType Units => UnitType.Milligram;
        public AnthraciteCoal(uint quantity) : base(quantity) { }
        override public float Width => .3f;
        private const string name = "Anthracite coal";
        public override string Name => name;
        public override string? ChemicalFormula => null;
        public override CombustionProperties? Combustion => properties;
        private static CombustionProperties properties = new()
        {
            BurnRateKgPerSecond = 0.0138888889f,
            CalorificValue_JoulesPerKg = 30_000_000
        };
    }
}