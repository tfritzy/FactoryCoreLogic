using System;
using System.Linq;
using Core;
using Newtonsoft.Json;

namespace Schema
{
    public class WornItemsComponent : InventoryComponent
    {
        public override ComponentType Type => ComponentType.WornItems;

        public override Core.Component FromSchema(object[] context)
        {
            Core.InventoryComponent inventory = (Core.InventoryComponent)base.FromSchema(context);
            return new Core.WornItemsComponent(inventory.Owner, inventory.GetCopyOfItems(), Width, Height);
        }
    }
}
