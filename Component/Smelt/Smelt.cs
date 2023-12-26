namespace Core
{
    // This thing
    //   - Accepts ore of dynamic type
    //   - Outputs ingot of the ore type
    //   - Can make alloys by having the right ores in its inventory
    //   - Does not accept non-ore items
    //   - Takes a dynamic amount of time te smelt, based on heat and ore type.
    //   - Can craft variants that smelt faster
    //   - Ingots are measured in kg. Smelting an ore type produces 
    //     ingots of size according to the ratio of elements.
    //   - Requires fuel. Some smelter types are powered by electricity and some
    //     by combustion. These two methods have different efficiencies, since induction
    //     transfers energy more efficiently than combustion.
    //   - Can intake fuel on a separate line, or through the main line.
    //   - Fuel is stored in a dedicated slot in the inventory.
    //   - Stops smelting if the output conveyor is full. This is awkward when the ore
    //   - produces multiple ingot types, since the conveyor accepts the ingots one at a time.
    //     maybe we can wait until the conveyor can accept at point 0, and then insert the others behind it.
    //   - Consumes fuel even when not smelting. This is to keep the smelter hot.
    //   - Combustion smelters cool down over time, and fuel is consumed when it gets too cold.
    //   - Induction smelters do not cool down, and use electricity in bursts.

    public class Smelt : Component
    {
        public float TemperatureCelsious { get; set; }
        public float HeatTransferCoefficient { get; private set; }
        public float SurfaceAreaSquareMeters { get; private set; }
        public float SpecificHeatCapacityJoulesPerKgCelsious { get; private set; }
        public float CombustionEfficiency { get; private set; }
        public float MassKg { get; private set; }
        public override ComponentType Type => ComponentType.Smelt;

        public Smelt(Entity owner) : base(owner)
        {
        }

        public override void Tick(float deltaTime)
        {
            base.Tick(deltaTime);
            CoolDown(deltaTime);
        }

        // Heat loss is calculated as ΔT = Q / (m * c)
        // Q is the amount of heat lost(in joules, J),
        // m is the mass of the material(in kilograms, kg),
        // c is the specific heat capacity of the material(in joules per kilogram per degree Celsius, J/kg°C),
        // 
        // Q (heat loss over time) is Q = h * A * (Tmaterial​ − Tenvironment​) * time
        // h is the heat transfer coefficient (in watts per square meter per degree Celsius, W/m²°C),
        // A is the surface area through which heat is being lost (in square meters, m²),
        // Tmaterial​ is the initial temperature of the material (in degrees Celsius, °C),
        // Tenvironment​ is the temperature of the surroundings (in degrees Celsius, °C),
        // t is the time over which heat is lost(in seconds, s).
        private void CoolDown(float deltaTime)
        {
            float heatLoss =
                HeatTransferCoefficient
                * SurfaceAreaSquareMeters
                * (TemperatureCelsious - Owner.Context.World.OutsideAirTemperatureCelsious)
                * deltaTime;
            float temperatureChange = heatLoss / (MassKg * SpecificHeatCapacityJoulesPerKgCelsious);
            TemperatureCelsious -= temperatureChange;
        }

        public void SetConstants(
            float heatTransferCoefficient,
            float surfaceAreaSquareMeters,
            float specificHeatCapacityJoulesPerKgCelsious,
            float massKg,
            float combustionEfficiency)
        {
            HeatTransferCoefficient = heatTransferCoefficient;
            SurfaceAreaSquareMeters = surfaceAreaSquareMeters;
            SpecificHeatCapacityJoulesPerKgCelsious = specificHeatCapacityJoulesPerKgCelsious;
            MassKg = massKg;
            CombustionEfficiency = combustionEfficiency;
        }

        public override Schema.Component ToSchema()
        {
            return new Schema.Smelt()
            {
                Heat = TemperatureCelsious,
            };
        }
    }
}