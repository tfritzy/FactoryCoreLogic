using System;
using System.Collections.Generic;
using Core;
using Newtonsoft.Json;

namespace Schema
{
    public class Life : Component
    {
        public override ComponentType Type => ComponentType.Life;

        [JsonProperty("health")]
        public int Health { get; set; }

        [JsonProperty("bHealth")]
        public int BaseHealth { get; set; }

        public override Core.Component FromSchema(object[] context)
        {
            if (context.Length == 0 || context[0] == null || !(context[0] is Core.Entity))
                throw new ArgumentException("LifeComponent requires an Entity as context[0]");

            Core.Entity owner = (Core.Entity)context[0];

            return new Core.Life(owner, BaseHealth, Health);
        }
    }
}
