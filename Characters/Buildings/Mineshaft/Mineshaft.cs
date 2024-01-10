using System.Collections.Generic;

namespace Core
{
    public class Mineshaft : Building
    {
        public override CharacterType Type => CharacterType.MineShaft;
        public override int Height => 3;
        private static readonly string name = "Mine";
        public override string Name => name;

        public Mineshaft(Context context, int alliance) : base(context, alliance) { }

        protected override void InitComponents()
        {
            this.SetComponent(new Mine(this));
            this.SetComponent(new Inventory(this, 4, 4));
            this.SetComponent(new ItemPort(this));
        }

        public override void ConfigureComponents()
        {
            base.ConfigureComponents();
            ItemPort!.OutputSideOffsets = new List<int> { 0 };
        }

        public new Schema.OneofCharacter ToSchema()
        {
            return new Schema.OneofCharacter
            {
                Mineshaft = new Schema.Mineshaft()
                {
                    Building = base.ToSchema(),
                }
            };
        }
    }
}
