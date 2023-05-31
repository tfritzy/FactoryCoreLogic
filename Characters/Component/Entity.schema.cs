using System;
using System.Collections.Generic;
using Core;
using Newtonsoft.Json;

namespace Schema
{
    public abstract class Entity
    {
        [JsonProperty("cmpts")]
        public Dictionary<Core.ComponentType, Component>? Components;

        [JsonProperty("id")]
        public ulong Id;

        [JsonProperty("entities")]
        public List<Entity>? ContainedEntities { get; set; }

        protected abstract Core.Entity BuildCoreObject(Context context);

        protected virtual Core.Entity CreateCore(params object[] context)
        {
            if (context.Length == 0 || !(context[0] is Core.Context))
                throw new InvalidOperationException("Context is missing.");

            Core.Entity entity = BuildCoreObject((Core.Context)context[0]);

            entity.Id = this.Id;
            if (this.ContainedEntities != null)
            {
                foreach (var contained in this.ContainedEntities)
                {
                    entity.AddContainedEntity(contained.CreateCore(context));
                }
            }

            return entity;
        }
    }
}