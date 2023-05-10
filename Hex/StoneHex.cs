using System;
using System.Collections.Generic;

namespace Core
{
    public class StoneHex : Hex
    {
        public override HexType Type => HexType.Stone;

        public StoneHex(Point3Int gridPosition, Context context) : base(gridPosition, context) { }

        protected override void InitComponents()
        {
            base.InitComponents();
            this.Components = new Dictionary<Type, Component>
            {
                { typeof(Harvestable), new Harvestable(this, HarvestableType.StoneHex) }
            };
        }

        public override Schema.Hex BuildSchemaObject()
        {
            return new Schema.StoneHex();
        }
    }
}