using System;
using Core;
using Newtonsoft.Json;

namespace Schema
{
    public class InventoryComponent : Component, Schema<Core.InventoryComponent>
    {
        public override ComponentType Type => ComponentType.Inventory;

        [JsonProperty("items")]
        public Item?[]? Items { get; set; }

        public Core.InventoryComponent FromSchema(object[] context)
        {
            if (context.Length == 0 || context[0] == null || !(context[0] is Core.Entity))
                throw new ArgumentException("InventoryComponent requires an Entity as context[0]");

            Core.Entity owner = (Core.Entity)context[0];

            if (Items == null)
                throw new ArgumentException("To build an InventoryComponent, Items must not be null.");

            return new Core.InventoryComponent(owner, Items);
        }
    }
}
