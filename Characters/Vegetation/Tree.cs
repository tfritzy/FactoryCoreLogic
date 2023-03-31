using System;
using System.Collections.Generic;

namespace Core
{
    public class Tree : Character
    {
        public override CharacterType Type => CharacterType.Tree;

        protected override void InitComponents()
        {
            this.Components = new Dictionary<Type, Component>
            {
                { typeof(HarvestableComponent), new HarvestableComponent(this, ItemType.Wood, 16, HarvestableType.Tree) }
            };
        }

        public Tree(Context context) : base(context)
        {
        }
    }
}
