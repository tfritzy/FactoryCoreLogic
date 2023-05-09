using System;
using System.Linq;
using Core;
using Newtonsoft.Json;

namespace Schema
{
    public class Inventory : Component
    {
        public override ComponentType Type => ComponentType.Inventory;

        [JsonProperty("items")]
        public Item?[]? Items { get; set; }

        [JsonProperty("w")]
        public int Width;

        [JsonProperty("h")]
        public int Height;

        public override Core.Component FromSchema(object[] context)
        {
            if (context.Length == 0 || context[0] == null || !(context[0] is Core.Entity))
                throw new ArgumentException("InventoryComponent requires an Entity as context[0]");

            Core.Entity owner = (Core.Entity)context[0];

            if (Items == null)
                throw new ArgumentException("To build an InventoryComponent, Items must not be null.");

            Core.Item?[] items = this.Items.Select(item => item?.FromSchema()).ToArray();

            return new Core.Inventory(owner, items, Width, Height);
        }
    }
}
