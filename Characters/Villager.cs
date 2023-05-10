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
            throw new System.NotImplementedException();
        }

        public Villager(Context context) : base(context)
        {
        }

        protected override void InitComponents()
        {
            this.Components = new Dictionary<Type, Component>
            {
                { typeof(Inventory), new Inventory(this, 3, 2) },
                { typeof(VillagerBehavior), new VillagerBehavior(this) },
            };
        }
    }
}