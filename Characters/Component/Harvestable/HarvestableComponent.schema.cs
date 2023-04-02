using System;
using System.Collections.Generic;
using Core;
using Newtonsoft.Json;
using static Core.HarvestableComponent;

namespace Schema
{
    public class HarvestableComponent : Component, Schema<Core.HarvestableComponent>
    {
        public override ComponentType Type => ComponentType.Harvestable;

        [JsonProperty("remainingItems")]
        public int RemainingItems { get; private set; }

        [JsonProperty("harvestableType")]
        public HarvestableType HarvestableType { get; private set; }

        public Core.HarvestableComponent FromSchema(object[] context)
        {
            if (context.Length == 0 || context[0] == null || !(context[0] is Core.Entity))
                throw new ArgumentException("HarvestableComponent requires an Core.Entity as context[0]");

            Core.Entity owner = (Core.Entity)context[0];

            var component = new Core.HarvestableComponent(owner, HarvestableType, RemainingItems);

            return component;
        }
    }
}
