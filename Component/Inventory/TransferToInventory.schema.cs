using System;
using System.Linq;
using Core;
using Newtonsoft.Json;

namespace Schema
{
    public class TransferToInventory : Inventory
    {
        public override ComponentType Type => ComponentType.TransferToInventory;

        public override Core.Component FromSchema(object[] context)
        {
            Core.Entity owner = (Core.Entity)context[0];
            return new Core.TransferToInventory(owner);
        }
    }
}
