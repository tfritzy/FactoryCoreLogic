
using System;
using System.Collections.Generic;
using System.Linq;

namespace Core
{
    public class WaterHex : Hex
    {
        public override HexType Type => HexType.Water;
        public override bool Transparent => true;

        public WaterHex(Point3Int gridPosition, Context context) : base(gridPosition, context) { }

        public override Schema.Entity BuildSchemaObject()
        {
            return new Schema.WaterHex();
        }
    }
}