using System;
using System.Collections.Generic;
using Core;
using Newtonsoft.Json;

namespace Schema
{
    public class Dummy : Character, ISchema<Core.Dummy>
    {
        [JsonProperty("remainingItems")]
        public int RemainingItems { get; private set; }

        [JsonProperty("harvestableType")]
        public HarvestableType HarvestableType { get; private set; }

        public Core.Dummy FromSchema(object[] context)
        {
            if (context.Length == 0 || context[0] == null || !(context[0] is Entity))
                throw new ArgumentException("Dummy requires a Context as context[0]");

            Context worldContext = (Context)context[0];

            var component = new Core.Dummy(worldContext);

            return component;
        }
    }
}
