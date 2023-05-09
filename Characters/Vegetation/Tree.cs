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
                { typeof(Harvestable), new Harvestable(this, HarvestableType.Tree) }
            };
        }

        public Tree(Context context) : base(context)
        {
        }

        public override Schema.Character ToSchema()
        {
            var tree = new Schema.Tree();
            return this.PopulateSchema(tree);
        }
    }
}
