using System;
using System.Collections.Generic;
using Core;
using Newtonsoft.Json;

namespace Schema
{
    public class Attack : Component
    {
        public override ComponentType Type => ComponentType.Attack;

        [JsonProperty("bDamage")]
        public int BaseDamage { get; set; }

        [JsonProperty("bCool")]
        public float BaseCooldown { get; set; }

        [JsonProperty("bRange")]
        public float BaseRange { get; set; }

        [JsonProperty("proj")]
        public ProjectileType Projectile { get; set; }


        public override Core.Component FromSchema(object[] context)
        {
            if (context.Length == 0 || context[0] == null || !(context[0] is Core.Entity))
                throw new ArgumentException("AttackComponent requires an Entity as context[0]");

            Core.Character owner = (Core.Character)context[0];

            return new Core.Attack(owner, BaseCooldown, BaseDamage, BaseRange, Projectile);
        }
    }
}
