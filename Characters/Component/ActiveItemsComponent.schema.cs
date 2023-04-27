using System;
using System.Linq;
using Core;
using Newtonsoft.Json;

namespace Schema
{
    public class ActiveItemsComponent : InventoryComponent
    {
        public override ComponentType Type => ComponentType.ActiveItems;

        public override Core.Component FromSchema(object[] context)
        {
            Core.InventoryComponent inventory = (Core.InventoryComponent)base.FromSchema(context);
            return new Core.ActiveItemsComponent(inventory.Owner, inventory.GetCopyOfItems(), Width, Height);
        }
    }
}
