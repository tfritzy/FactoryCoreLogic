using System;
using System.Linq;
using Core;
using Newtonsoft.Json;

namespace Schema
{
    public class WornItems : Inventory
    {
        public override ComponentType Type => ComponentType.WornItems;

        public override Core.Component FromSchema(object[] context)
        {
            Core.Inventory inventory = (Core.Inventory)base.FromSchema(context);
            return new Core.WornItems(inventory.Owner, inventory.GetCopyOfItems(), Width, Height);
        }
    }
}
