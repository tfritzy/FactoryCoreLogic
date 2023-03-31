using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Schema
{
    [JsonConverter(typeof(CharacterConverter))]
    public abstract class Entity
    {
        [JsonProperty("components")]
        protected Dictionary<Type, Component>? Components;

        [JsonProperty("id")]
        public ulong Id;
    }
}