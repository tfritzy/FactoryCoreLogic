using System;

namespace Schema
{
    public class DirtHex : Hex
    {
        public override Core.HexType Type => Core.HexType.Dirt;

        public override Core.Hex FromSchema(params object[] context)
        {
            return this.CreateCore(context);
        }
    }
}