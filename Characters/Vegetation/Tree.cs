using System;
using System.Collections.Generic;
using System.Linq;

namespace Core
{
    public class Tree : Vegetation
    {
        public override VegetationType Type => VegetationType.Tree;
        private static Point2Float defaultOffset = new Point2Float(0, 0);

        public Tree(Context context, Point2Float? positionOffset = null) : base(context, positionOffset ?? defaultOffset)
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
