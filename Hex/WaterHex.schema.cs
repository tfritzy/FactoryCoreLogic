using System;

namespace Schema
{
    public class WaterHex : Hex
    {
        public override Core.HexType Type => Core.HexType.Water;

        public override Core.Hex FromSchema(params object[] context)
        {
            return this.CreateCore(context);
        }
    }
}