
using System;
using System.Collections.Generic;

namespace FactoryCore
{
    public class DirtHex : Hex
    {
        public override HexType Type => HexType.Dirt;

        protected override void InitCells()
        {
            base.InitCells();
            this.Cells = new Dictionary<Type, Cell>() {
                {typeof(Harvestable), new Harvestable(this, ItemType.Dirt, 8, HarvestableType.DirtHex)}
            };
        }

        public DirtHex(Point3Int gridPosition, Context context) : base(gridPosition, context)
        {
        }
    }
}