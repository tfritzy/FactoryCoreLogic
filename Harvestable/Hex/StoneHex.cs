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

        protected override void InitCells()
        {
            base.InitCells();
            this.Cells = new Dictionary<Type, Cell>
            {
                { typeof(HarvestableCell), new HarvestableCell(this, ItemType.Stone, 8, HarvestableType.StoneHex) }
            };
        }
    }
}