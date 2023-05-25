using System;
using System.Collections.Generic;
using Core;
using Newtonsoft.Json;

namespace Schema
{
    public class Harvest : Component
    {
        public override ComponentType Type => ComponentType.Harvester;

        [JsonProperty("hRates")]
        public Dictionary<HarvestableType, float>? HarvestRateSeconds { get; set; }

        public override Core.Component FromSchema(object[] context)
        {
            if (context.Length == 0 || context[0] == null || !(context[0] is Core.Entity))
                throw new ArgumentException("HarvestComponent requires an Entity as context[0]");

            if (HarvestRateSeconds == null)
                throw new ArgumentException("To build a HarvestComponent, HarvestRateSeconds must be set");

            Core.Entity owner = (Core.Entity)context[0];

            var component = new Core.Harvester(owner);

            component.HarvestRateSeconds = HarvestRateSeconds;

            return component;
        }
    }
}
