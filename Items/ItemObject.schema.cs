using Newtonsoft.Json;

namespace Schema
{
    public class ItemObject
    {
        [JsonProperty("item")]
        public Item Item { get; set; }

        [JsonProperty("position")]
        public Core.Point3Float Position { get; set; }

        [JsonProperty("rotation")]
        public Core.Point3Float Rotation { get; set; }

        public ItemObject()
        {
        }

        public Core.ItemObject FromSchema(params object[] context)
        {
            return new Core.ItemObject(Item.FromSchema(context), Position, Rotation);
        }
    }
}