using System;

namespace Schema
{
    public class StoneHex : Hex
    {
        public override Core.HexType Type => Core.HexType.Stone;

        public override Core.Hex FromSchema(params object[] context)
        {
            return this.CreateCore(context);
        }
    }
}