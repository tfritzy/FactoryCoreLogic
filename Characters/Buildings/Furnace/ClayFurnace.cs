namespace Core
{
    public class ClayFurnace : Building
    {

        public override int Height => 2;
        public override CharacterType Type => CharacterType.ClayFurnace;
        public override string Name => "Clay Furnace";

        public ClayFurnace(Context context, int alliance) : base(context, alliance)
        {
        }

        protected override void InitComponents()
        {
            base.InitComponents();
            SetComponent(new Smelt(this));
            SetComponent(new ItemPort(this));
            SetComponent(new Inventory(this, 2, 1));
        }

        public override void ConfigureComponents()
        {
            base.ConfigureComponents();
            Smelt!.SetConstants(
                heatTransferCoefficient: Constants.HeatTransferCoefficient.FireclayBrick,
                specificHeatCapacityJoulesPerKgCelsious: Constants.SpecificHeatCapacity.FireclayBrick,
                surfaceAreaSquareMeters: 2f,
                massKg: 100f
            );
        }

        public override Schema.Entity BuildSchemaObject()
        {
            return new Schema.ClayFurnace();
        }
    }
}