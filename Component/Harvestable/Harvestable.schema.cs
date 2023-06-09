using System;
using Core;
using Newtonsoft.Json;

namespace Schema
{
    public class Harvestable : Component
    {
        public override ComponentType Type => ComponentType.Harvestable;

        [JsonProperty("remItms")]
        public int RemainingItems { get; set; }

        [JsonProperty("hType")]
        public HarvestableType HarvestableType { get; set; }

        public override Core.Component FromSchema(object[] context)
        {
            if (context.Length == 0 || context[0] == null || !(context[0] is Core.Entity))
                throw new ArgumentException("HarvestableComponent requires an Core.Entity as context[0]");

            Core.Entity owner = (Core.Entity)context[0];

            return new Core.Harvestable(owner, HarvestableType)
            {
                RemainingItems = RemainingItems
            };
        }
    }
}
