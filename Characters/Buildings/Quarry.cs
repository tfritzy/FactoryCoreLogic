using System.Collections.Generic;

namespace Core
{
    public class Quarry : Building
    {
        public override CharacterType Type => CharacterType.Quarry;

        public Quarry(Context context, int alliance) : base(context, alliance)
        {
        }

        public override Schema.Character ToSchema()
        {
            var quarry = new Schema.Quarry();
            return this.PopulateSchema(quarry);
        }

        protected override void InitComponents()
        {
            this.SetComponent(new Inventory(this, 3, 2));
            this.SetComponent(new QuarryWorksite(this));
        }
    }
}