
using System;
using System.Collections.Generic;

namespace Core
{
    public class DirtHex : Hex
    {
        public override HexType Type => HexType.Dirt;

        protected override void InitComponents()
        {
            base.InitComponents();
            this.Components = new Dictionary<Type, Component>() {
                {typeof(HarvestableComponent), new HarvestableComponent(this, ItemType.Dirt, 8, HarvestableType.DirtHex)}
            };
        }

        public DirtHex(Point3Int gridPosition, Context context) : base(gridPosition, context)
        {
        }
    }
}