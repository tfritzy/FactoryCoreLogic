using System;
using System.Linq;
using Core;
using Newtonsoft.Json;

namespace Schema
{
    public class WornItems : InventoryComponent
    {
        public override ComponentType Type => ComponentType.WornItems;

        public override Core.Component FromSchema(object[] context)
        {
            Core.InventoryComponent inventory = (Core.InventoryComponent)base.FromSchema(context);
            return new Core.WornItems(inventory.Owner, inventory.GetCopyOfItems(), Width, Height);
        }
    }
}
