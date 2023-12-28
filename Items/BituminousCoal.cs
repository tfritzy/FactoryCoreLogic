namespace Core
{
    public class BituminousCoal : Item
    {
        public override ItemType Type => ItemType.BituminousCoal;
        public override uint MaxStack => 200_000_000;
        public override UnitType Units => UnitType.Milligram;
        public BituminousCoal(uint quantity) : base(quantity) { }
        override public float Width => .3f;
        private const string name = "Bituminous coal";
        public override string Name => name;
        public override string? ChemicalFormula => null;
        public override CombustionProperties? Combustion => properties;
        private static CombustionProperties properties = new()
        {
            BurnRateMilligramPerSecond = 14000,
            CalorificValue_JoulesPerMg = 24
        };
    }
}