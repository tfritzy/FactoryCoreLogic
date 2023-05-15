using System;
using System.Collections.Generic;

namespace Core
{
    public class Villager : Character
    {
        public override CharacterType Type => CharacterType.Villager;
        public VillagerBehavior Behavior => this.GetComponent<VillagerBehavior>();

        public override Schema.Character ToSchema()
        {
            var villager = new Schema.Villager();
            return this.PopulateSchema(villager);
        }

        public Villager(Context context) : base(context)
        {
        }

        protected override void InitComponents()
        {
            this.SetComponent(new Inventory(this, 3, 2));
            this.SetComponent(new VillagerBehavior(this));
        }
    }
}