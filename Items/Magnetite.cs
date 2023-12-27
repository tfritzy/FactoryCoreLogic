namespace Core
{
    public class Magnetite : Item
    {
        public override ItemType Type => ItemType.Magnetite;
        public override string Name => "Magnetite";
        public override string? ChemicalFormula => "Fe₃O₄";
        public override float? SpecificHeat_JoulesPerKgPerDegreeCelsious => 700;
        public override float? MeltingPoint_Celsious => 1590;
    }
}