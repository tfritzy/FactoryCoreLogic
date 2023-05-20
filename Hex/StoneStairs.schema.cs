using System;

namespace Schema
{
    public class StoneStairs : Hex
    {
        public override Core.HexType Type => Core.HexType.StoneStairs;

        public override Core.Hex FromSchema(params object[] context)
        {
            return this.CreateCore(context);
        }
    }
}