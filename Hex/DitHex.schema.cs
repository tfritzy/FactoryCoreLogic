using System;

namespace Schema
{
    public class DirtHex : Hex
    {
        public override Core.HexType Type => Core.HexType.Dirt;

        public override Core.Hex FromSchema(params object[] context)
        {
            if (this.GridPosition == null)
                throw new InvalidOperationException("GridPosition is null.");

            if (context.Length == 0 || !(context[0] is Core.Context))
                throw new InvalidOperationException("Context is missing.");

            return new Core.DirtHex(this.GridPosition.Value, (Core.Context)context[0]);
        }
    }
}