using System;
using System.Collections.Generic;

namespace Core
{
    public class Villiger : Character
    {
        public override CharacterType Type => CharacterType.Villiger;
        public VilligerBehavior Behavior => this.GetComponent<VilligerBehavior>();

        public override Schema.Character ToSchema()
        {
            throw new System.NotImplementedException();
        }

        public Villiger(Context context) : base(context)
        {
        }

        protected override void InitComponents()
        {
            this.Components = new Dictionary<Type, Component>
            {
                { typeof(Inventory), new Inventory(this, 3, 2) },
                { typeof(VilligerBehavior), new VilligerBehavior(this) },
            };
        }
    }
}