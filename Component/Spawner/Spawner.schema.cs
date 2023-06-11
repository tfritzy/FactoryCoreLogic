using System.Collections.Generic;
using Core;
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

        [JsonProperty("spawnableTypes")]
        public List<CharacterType> SpawnableTypes { get; set; } = new List<CharacterType>();

        public override Core.Component FromSchema(params object[] context)
        {
            return new Core.Spawner(
                owner: (Core.Entity)context[0],
                range: Range,
                power: Power,
                powerAccrualRate: PowerAccrualRate,
                spawnableTypes: SpawnableTypes);
        }
    }
}