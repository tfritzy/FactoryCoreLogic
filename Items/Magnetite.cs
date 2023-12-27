namespace Core
{
    public class Magnetite : Ore
    {
        public override ItemType Type => ItemType.Magnetite;
        public override string Name => "Magnetite";
        public override string? ChemicalFormula => "Fe₃O₄";
        public override float? SpecificHeat_JoulesPerKgPerDegreeCelsious => 700;
        public override float? MeltingPoint_Celsious => 1590;
        public override uint MaxStack => 50_000_000;
        public override UnitType Units => UnitType.Milligram;

        public Magnetite(uint quantity) : base(quantity) { }
    }
}