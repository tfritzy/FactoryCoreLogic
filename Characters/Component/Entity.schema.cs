using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Schema
{
    public abstract class Entity
    {
        [JsonProperty("cmpts")]
        public Dictionary<Core.ComponentType, Component>? Components;

        [JsonProperty("id")]
        public ulong Id;
    }
}