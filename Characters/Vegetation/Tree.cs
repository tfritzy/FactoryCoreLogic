using System;
using System.Collections.Generic;
using System.Linq;

namespace Core
{
    public class Tree : Character
    {
        public override CharacterType Type => CharacterType.Tree;

        protected override void InitComponents()
        {
            this.Components = new Dictionary<Type, Component>
            {
                { typeof(HarvestableComponent), new HarvestableComponent(this, HarvestableType.Tree) }
            };
        }

        public Tree(Context context) : base(context)
        {
        }

        public override Schema.Tree ToSchema()
        {
            return new Schema.Tree()
            {
                Id = this.Id,
                GridPosition = this.GridPosition,
                Components = this.Components.ToDictionary(
                    x => Component.ComponentTypeMap[x.Key], x => x.Value.ToSchema())
            };
        }
    }
}
