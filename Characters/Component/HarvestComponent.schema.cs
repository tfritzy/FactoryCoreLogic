using System;
using System.Collections.Generic;
using Core;
using Newtonsoft.Json;

namespace Schema
{
    public class HarvestComponent : Component
    {
        public override ComponentType Type => ComponentType.Harvest;

        [JsonProperty("harvestTargetId")]
        public ulong? HarvestTargetId { get; set; }

        [JsonProperty("targetHarvestPoint")]
        public Point3Int? TargetHarvestPoint { get; set; }

        [JsonProperty("tillH")]
        public float? TimeUntilHarvest { get; set; }

        [JsonProperty("hRates")]
        public Dictionary<HarvestableType, float>? HarvestRateSeconds { get; set; }

        public override Core.Component FromSchema(object[] context)
        {
            if (context.Length == 0 || context[0] == null || !(context[0] is Core.Entity))
                throw new ArgumentException("HarvestComponent requires an Entity as context[0]");

            if (HarvestRateSeconds == null)
                throw new ArgumentException("To build a HarvestComponent, HarvestRateSeconds must be set");

            Core.Entity owner = (Core.Entity)context[0];

            var component = new Core.HarvestComponent(owner);

            component.HarvestRateSeconds = HarvestRateSeconds;

            if (HarvestTargetId != null)
                component.SetTarget(HarvestTargetId.Value);
            else if (TargetHarvestPoint != null)
                component.SetTarget(TargetHarvestPoint.Value);

            if (TimeUntilHarvest != null)
            {
                component.SetTimeUntilHarvest(this.TimeUntilHarvest.Value);
            }

            return component;
        }
    }
}
