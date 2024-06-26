namespace Core
{
    public class Chalcopyrite : Ore
    {
        public override ItemType Type => ItemType.Chalcopyrite;
        public override string Name => "Chalcopyrite";
        public override string ChemicalFormula => "CuFeS₂";
        public override float? SpecificHeat_JoulesPerMgPerDegreeCelsious => .0004f;
        public override float? MeltingPoint_Celsious => 950;
        public override ulong MaxStack => 200_000_000;
        public override UnitType Units => UnitType.Milligram;
        public Chalcopyrite(ulong quantity) : base(quantity) { }
    }
}