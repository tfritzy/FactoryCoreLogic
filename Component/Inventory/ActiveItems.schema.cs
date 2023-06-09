using System;
using System.Linq;
using Core;
using Newtonsoft.Json;

namespace Schema
{
    public class ActiveItems : Inventory
    {
        public override ComponentType Type => ComponentType.ActiveItems;

        public override Core.Component FromSchema(object[] context)
        {
            Core.Inventory inventory = (Core.Inventory)base.FromSchema(context);
            return new Core.ActiveItems(inventory.Owner, inventory.GetCopyOfItems(), Width, Height);
        }
    }
}
