using System;
using Core;
using Newtonsoft.Json;

namespace Schema
{
    public class TowerTargeting : Component
    {
        public override ComponentType Type => ComponentType.TowerTargeting;

        [JsonProperty("mode")]
        public TowerTargetingMode Mode;

        public override Core.Component FromSchema(object[] context)
        {
            if (context.Length == 0 || context[0] == null || !(context[0] is Core.Entity))
                throw new ArgumentException("TowerTargetingComponent requires an Entity as context[0]");

            Core.Entity owner = (Core.Entity)context[0];

            var component = new Core.TowerTargeting(owner);
            component.SetMode(Mode);

            return component;
        }
    }
}
