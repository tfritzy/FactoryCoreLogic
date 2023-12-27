namespace Core
{
    public class LigniteCoal : Item
    {
        public override ItemType Type => ItemType.LigniteCoal;
        public LigniteCoal(uint quantity) : base(quantity) { }
        override public float Width => .3f;
        private const string name = "Lignite coal";
        public override string Name => name;
        public override string? ChemicalFormula => null;
        public override CombustionProperties? Combustion => properties;
        public override uint MaxStack => 50_000_000;
        public override UnitType Units => UnitType.Milligram;
        private static CombustionProperties properties = new()
        {
            BurnRateKgPerSecond = 1f,
            CalorificValue_JoulesPerKg = 12_000_000
        };
    }
}