namespace Core
{
    public class BituminousCoal : Item
    {
        public override ItemType Type => ItemType.BituminousCoal;
        public override int MaxStack => 16;
        public BituminousCoal(int quantity) : base(quantity) { }
        public BituminousCoal() : base() { }
        override public float Width => .3f;
        private const string name = "Bituminous coal";
        public override string Name => name;
        public override string? ChemicalFormula => null;
        public override CombustionProperties? Combustion => properties;
        private static CombustionProperties properties = new()
        {
            BurnRateKgPerHr = .8f,
            CalorificValue_JoulesPerKg = 24_000_000
        };
    }
}