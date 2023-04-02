using Newtonsoft.Json;

namespace Schema
{
    public class Item : SchemaOf<Core.Item>
    {
        [JsonProperty("type")]
        public Core.ItemType Type { get; set; }

        [JsonProperty("quantity")]
        public int Quantity { get; set; }

        public Core.Item FromSchema(params object[] context)
        {
            Core.Item item = Core.Item.Create(Type);
            item.SetQuantity(Quantity);
            return item;
        }
    }
}