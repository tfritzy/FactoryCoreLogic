using Newtonsoft.Json;

namespace Schema
{
    public class Smelt : Component
    {
        public override Core.ComponentType Type => Core.ComponentType.Smelt;

        [JsonProperty(nameof(Heat))]
        public float Heat { get; set; }

        public override Core.Component FromSchema(params object[] context)
        {
            var smelt = new Core.Smelt((Core.Building)context[0])
            {
                TemperatureCelsious = Heat
            };
            return smelt;
        }
    }
}