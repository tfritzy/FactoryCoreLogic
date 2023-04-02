using System;
using System.Collections.Generic;
using Core;
using Newtonsoft.Json;
using static Core.HarvestableComponent;

namespace Schema
{
    public class HarvestableComponent : Component
    {
        public override ComponentType Type => ComponentType.Harvestable;

        [JsonProperty("remItms")]
        public int RemainingItems { get; set; }

        [JsonProperty("hType")]
        public HarvestableType HarvestableType { get; set; }

        public override Core.HarvestableComponent FromSchema(object[] context)
        {
            if (context.Length == 0 || context[0] == null || !(context[0] is Core.Entity))
                throw new ArgumentException("HarvestableComponent requires an Core.Entity as context[0]");

            Core.Entity owner = (Core.Entity)context[0];

            return new Core.HarvestableComponent(owner, HarvestableType)
            {
                RemainingItems = RemainingItems
            };
        }
    }
}
