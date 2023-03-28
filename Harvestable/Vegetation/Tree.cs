using System;
using System.Collections.Generic;

namespace FactoryCore
{
    public class Tree : Character
    {
        public override CharacterType Type => CharacterType.Tree;

        protected override void InitCells()
        {
            this.Cells = new Dictionary<Type, Cell>
            {
                { typeof(HarvestableCell), new HarvestableCell(this, ItemType.Wood, 16, HarvestableType.Tree) }
            };
        }

        public Tree(Context context) : base(context)
        {
        }
    }
}
