using System;
using System.Collections.Generic;
using Core;
using Newtonsoft.Json;

namespace Schema
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public abstract class Entity
    {
        [JsonProperty("cmpts")]
        public List<Component>? Components;

        [JsonProperty("id")]
        public ulong Id;

        protected abstract Core.Entity BuildCoreObject(Context context);

        protected virtual Core.Entity CreateCore(params object[] context)
        {
            if (context.Length == 0 || !(context[0] is Core.Context))
                throw new InvalidOperationException("Context is missing.");

            Core.Entity entity = BuildCoreObject((Core.Context)context[0]);
            entity.Id = this.Id;

            if (Components != null)
            {
                foreach (var component in this.Components)
                {
                    Core.Component deser = component.FromSchema(entity);
                    entity.SetComponent(deser);
                }
            }
            entity.ConfigureComponents();

            return entity;
        }
    }
}