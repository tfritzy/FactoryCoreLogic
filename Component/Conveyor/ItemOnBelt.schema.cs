using Core;
using Newtonsoft.Json;

namespace Schema
{
    public class ItemOnBelt
    {
        [JsonProperty("item")]
        public SchemaItem? Item { get; set; }

        [JsonProperty("progressMeters")]
        public float? ProgressMeters { get; set; }

        public Core.ItemOnBelt FromSchema(params object[] context)
        {
            if (Item == null)
                throw new System.ArgumentException("To build an ItemOnBelt, Item must not be null.");

            if (ProgressMeters == null)
                throw new System.ArgumentException("To build an ItemOnBelt, ProgressMeters must not be null.");

            return new Core.ItemOnBelt(Core.Item.FromSchema(Item), ProgressMeters.Value);
        }
    }
}