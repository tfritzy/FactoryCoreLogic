using System;
using Core;
using Newtonsoft.Json;

namespace Schema
{
    public class HarvestComponent : Component, Schema<Core.HarvestComponent>
    {
        public override ComponentType Type => ComponentType.Harvest;

        [JsonProperty("harvestTargetId")]
        public ulong? HarvestTargetId { get; private set; }

        [JsonProperty("targetHarvestPoint")]
        public Point3Int? TargetHarvestPoint { get; private set; }

        public Core.HarvestComponent FromSchema(object[] context)
        {
            if (context.Length == 0 || context[0] == null || !(context[0] is Core.Entity))
                throw new ArgumentException("HarvestComponent requires an Entity as context[0]");

            Core.Entity owner = (Core.Entity)context[0];

            var component = new Core.HarvestComponent(owner);

            if (HarvestTargetId != null)
                component.SetTarget(HarvestTargetId.Value);
            else if (TargetHarvestPoint != null)
                component.SetTarget(TargetHarvestPoint.Value);

            return component;
        }
    }
}
