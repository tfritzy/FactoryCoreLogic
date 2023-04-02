using Newtonsoft.Json;

namespace Schema
{
    public class ItemOnBelt : SchemaOf<Core.ItemOnBelt>
    {
        [JsonProperty("item")]
        public Item? Item { get; set; }

        [JsonProperty("progressMeters")]
        public float? ProgressMeters { get; set; }

        public Core.ItemOnBelt FromSchema(params object[] context)
        {
            if (Item == null)
                throw new System.ArgumentException("To build an ItemOnBelt, Item must not be null.");

            if (ProgressMeters == null)
                throw new System.ArgumentException("To build an ItemOnBelt, ProgressMeters must not be null.");

            return new Core.ItemOnBelt(Item.FromSchema(), ProgressMeters.Value);
        }
    }
}