using System;

namespace Schema
{
    public class Bedrock : Hex
    {
        public override Core.HexType Type => Core.HexType.Bedrock;

        public override Core.Hex FromSchema(params object[] context)
        {
            return this.CreateCore(context);
        }
    }
}