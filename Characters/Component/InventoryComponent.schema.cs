using System;
using FactoryCore;
using Newtonsoft.Json;

namespace Schema
{
    public class InventoryComponent : Component, ISchema<FactoryCore.InventoryComponent>
    {
        [JsonProperty("items")]
        public Item?[]? Items { get; set; }

        public FactoryCore.InventoryComponent FromSchema(object[] context)
        {
            if (context.Length == 0 || context[0] == null || !(context[0] is Entity))
                throw new ArgumentException("InventoryComponent requires an Entity as context[0]");

            Entity owner = (Entity)context[0];

            if (Items == null)
                throw new ArgumentException("To build an InventoryComponent, Items must not be null.");

            return new FactoryCore.InventoryComponent(owner, Items);
        }
    }
}
