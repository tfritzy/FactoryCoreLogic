using System;
using System.Collections.Generic;
using System.Linq;

namespace Core
{
    public class Tree : Vegetation
    {
        public override VegetationType Type => VegetationType.Tree;

        public Tree(Context context) : base(context)
        {
        }

        protected override void InitComponents()
        {
            this.SetComponent(new Harvestable(this, HarvestableType.Tree));
        }

        public override Schema.Entity BuildSchemaObject()
        {
            return new Schema.Tree();
        }
    }
}
