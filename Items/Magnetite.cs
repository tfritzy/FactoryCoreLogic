namespace Core
{
    public class Magnetite : Ore
    {
        public override ItemType Type => ItemType.Magnetite;
        public override string Name => "Magnetite";
        public override string? ChemicalFormula => "Fe₃O₄";
        public override float? SpecificHeat_JoulesPerMgPerDegreeCelsious => .0007f;
        public override float? MeltingPoint_Celsious => 1590;
        public override uint MaxStack => 200_000_000;
        public override UnitType Units => UnitType.Milligram;

        public Magnetite(ulong quantity) : base(quantity) { }
    }
}