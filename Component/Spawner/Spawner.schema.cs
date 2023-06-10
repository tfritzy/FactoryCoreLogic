using Newtonsoft.Json;

namespace Schema
{
    public class Spawner : Component
    {
        public override Core.ComponentType Type => Core.ComponentType.Spawner;

        [JsonProperty("range")]
        public int Range { get; set; }

        [JsonProperty("accrlRate")]
        public float PowerAccrualRate { get; set; }

        [JsonProperty("power")]
        public float Power { get; set; }

        public override Core.Component FromSchema(params object[] context)
        {
            return new Core.Spawner((Core.Entity)context[0], Range, PowerAccrualRate, Power);
        }
    }
}