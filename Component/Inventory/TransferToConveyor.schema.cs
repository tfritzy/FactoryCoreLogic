using System;
using System.Linq;
using Core;
using Newtonsoft.Json;

namespace Schema
{
    public class TransferToConveyor : Inventory
    {
        public override ComponentType Type => ComponentType.TransferToConveyor;

        public override Core.Component FromSchema(object[] context)
        {
            Core.Entity owner = (Core.Entity)context[0];
            return new Core.TransferToConveyor(owner);
        }
    }
}
