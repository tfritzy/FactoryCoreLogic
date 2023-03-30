using System;
using FactoryCore;
using Newtonsoft.Json;

namespace Schema
{
    public class HarvestComponent : Component, ISchema<FactoryCore.HarvestComponent>
    {
        [JsonProperty("harvestTargetId")]
        public ulong? HarvestTargetId { get; private set; }

        [JsonProperty("targetHarvestPoint")]
        public Point3Int? TargetHarvestPoint { get; private set; }

        public FactoryCore.HarvestComponent FromSchema(object[] context)
        {
            if (context.Length == 0 || context[0] == null || !(context[0] is Entity))
                throw new ArgumentException("HarvestComponent requires an Entity as context[0]");

            Entity owner = (Entity)context[0];

            var component = new FactoryCore.HarvestComponent(owner);

            if (HarvestTargetId != null)
                component.SetTarget(HarvestTargetId.Value);
            else if (TargetHarvestPoint != null)
                component.SetTarget(TargetHarvestPoint.Value);

            return component;
        }
    }
}
