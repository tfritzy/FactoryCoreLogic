using System;
using System.Collections.Generic;

namespace Core
{
    public class Villager : Unit
    {
        public override CharacterType Type => CharacterType.Villager;
        public VillagerBehavior Behavior => this.GetComponent<VillagerBehavior>();

        public override Point3Int GridPosition => throw new NotImplementedException();

        public Villager(Context context, int alliance) : base(context, alliance)
        {
        }

        public override Schema.Entity BuildSchemaObject()
        {
            return new Schema.Villager();
        }

        protected override void InitComponents()
        {
            this.SetComponent(new Inventory(this, 3, 2));
            this.SetComponent(new VillagerBehavior(this));
        }
    }
}