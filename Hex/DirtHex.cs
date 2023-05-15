
using System;
using System.Collections.Generic;
using System.Linq;

namespace Core
{
    public class DirtHex : Hex
    {
        public override HexType Type => HexType.Dirt;

        public DirtHex(Point3Int gridPosition, Context context) : base(gridPosition, context) { }

        protected override void InitComponents()
        {
            base.InitComponents();
            this.SetComponent(new Harvestable(this, HarvestableType.DirtHex));
        }

        public override Schema.Hex BuildSchemaObject()
        {
            return new Schema.DirtHex();
        }
    }
}