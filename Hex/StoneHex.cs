using System;
using System.Collections.Generic;

namespace FactoryCore
{
    public class StoneHex : Hex
    {
        public override HexType Type => HexType.Stone;

        public StoneHex(Point3Int gridPosition, Context context) : base(gridPosition, context)
        {
        }

        protected override void InitComponents()
        {
            base.InitComponents();
            this.Cells = new Dictionary<Type, Component>
            {
                { typeof(HarvestableComponent), new HarvestableComponent(this, ItemType.Stone, 8, HarvestableType.StoneHex) }
            };
        }
    }
}