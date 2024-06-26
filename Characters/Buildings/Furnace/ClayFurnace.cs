using System.Runtime.InteropServices;

namespace Core
{
    public class ClayFurnace : Building
    {

        public override int Height => 2;
        public override CharacterType Type => CharacterType.ClayFurnace;
        public override string Name => "Clay Furnace";

        public ClayFurnace(Context context, Schema.ClayFurnace clayFurnace) : base(clayFurnace.Building, context) { }
        public ClayFurnace(Context context, int alliance) : base(context, alliance) { }

        protected override void InitComponents()
        {
            base.InitComponents();
            SetComponent(new Smelt(this));
            SetComponent(new ItemPort(this));
            SetComponent(new Inventory(this, 2, 1));
            SetComponent(new OreInventory(this, 1, 1));
            SetComponent(new FuelInventory(this, 1, 1));
        }

        public override void ConfigureComponents()
        {
            base.ConfigureComponents();
            Smelt!.SetConstants(
                combustionEfficiency: .55f
            );
            ItemPort!.InputSideOffsets = new() { 1, 2, 3, 4, 5 };
            ItemPort!.OutputSideOffsets = new() { 0 };
            ItemPort!.GetDestinationForItem = Smelt.GetItemDirectionFunction(this);
        }

        public override Schema.OneofCharacter Serialize()
        {
            return new Schema.OneofCharacter
            {
                ClayFurnace = new Schema.ClayFurnace()
                {
                    Building = base.ToSchema(),
                }
            };
        }
    }
}