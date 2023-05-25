using System;
using System.Linq;
using Core;
using Newtonsoft.Json;

namespace Schema
{
    public class VillagerBehavior : Component
    {
        public override ComponentType Type => ComponentType.VillagerBehavior;

        [JsonProperty("EmplAt")]
        public ulong? BuildingEmployedAt { get; set; }

        public override Core.Component FromSchema(params object[] context)
        {
            if (context.Length == 0 || context[0] == null || !(context[0] is Core.Entity))
                throw new ArgumentException("VillagerBehavior requires an Entity as context[0]");

            Core.Entity owner = (Core.Entity)context[0];
            var component = new Core.VillagerBehavior(owner);
            component.SetPlaceOfEmployment(BuildingEmployedAt);

            return component;
        }
    }
}
