
using System;
using System.Collections.Generic;
using System.Linq;

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


        public override Schema.Hex ToSchema()
        {
            return new Schema.DirtHex
            {
                ContainedEntities = this.ContainedEntities,
                GridPosition = this.GridPosition,
                Id = this.Id,
            };
        }
    }
}