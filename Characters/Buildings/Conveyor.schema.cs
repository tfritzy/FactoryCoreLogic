using System;
using System.Collections.Generic;
using Core;
using Newtonsoft.Json;
using static Core.Conveyor;

namespace Schema
{
    public class Conveyor : Component, ISchema<Core.Conveyor>
    {
        [JsonProperty("remainingItems")]
        public int RemainingItems { get; private set; }

        [JsonProperty("harvestableType")]
        public HarvestableType HarvestableType { get; private set; }

        public Core.Conveyor FromSchema(object[] context)
        {
            if (context.Length == 0 || context[0] == null || !(context[0] is Entity))
                throw new ArgumentException("Conveyor requires a Context as context[0]");

            Context worldContext = (Context)context[0];

            var component = new Core.Conveyor(worldContext);

            return component;
        }
    }
}
