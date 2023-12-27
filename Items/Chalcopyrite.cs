namespace Core
{
    public class Chalcopyrite : Item
    {
        public override ItemType Type => ItemType.Chalcopyrite;
        public override string Name => "Chalcopyrite";
        public override string ChemicalFormula => "CuFeS₂";
        public override float? SpecificHeat_JoulesPerKgPerDegreeCelsious => 400;
        public override float? MeltingPoint_Celsious => 950;

        public Chalcopyrite(int quantity) : base(quantity) { }
        public Chalcopyrite() { }
    }
}