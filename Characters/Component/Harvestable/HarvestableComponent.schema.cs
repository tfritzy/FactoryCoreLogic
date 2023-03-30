using System;
using System.Collections.Generic;
using FactoryCore;
using Newtonsoft.Json;
using static FactoryCore.HarvestableComponent;

namespace Schema
{
    public class HarvestableComponent : Component, ISchema<FactoryCore.HarvestableComponent>
    {
        [JsonProperty("remainingItems")]
        public int RemainingItems { get; private set; }

        [JsonProperty("harvestableType")]
        public HarvestableType HarvestableType { get; private set; }

        public FactoryCore.HarvestableComponent FromSchema(object[] context)
        {
            if (context.Length == 0 || context[0] == null || !(context[0] is Entity))
                throw new ArgumentException("HarvestableComponent requires an Entity as context[0]");

            Entity owner = (Entity)context[0];

            var component = new FactoryCore.HarvestableComponent(owner, HarvestableType, RemainingItems);

            return component;
        }
    }
}
