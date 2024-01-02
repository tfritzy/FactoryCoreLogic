using System;
using Core;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Schema
{
    public class Item : SchemaOf<Core.Item>
    {
        [JsonProperty("type")]
        public Core.ItemType Type { get; set; }

        [JsonProperty("quantity")]
        public ulong Quantity { get; set; }

        [JsonProperty("id")]
        public ulong Id { get; set; }

        public Core.Item FromSchema(params object[] context)
        {
            Core.Item item = Core.Item.Create(Type);
            item.SetQuantity(Quantity);
            item.Id = Id;
            return item;
        }
    }
}