using System;
using System.Collections.Generic;
using Core;
using Newtonsoft.Json;

namespace Schema
{
    public class Terrain : SchemaOf<Core.Terrain>
    {
        [JsonProperty("terrain")]
        public Triangle?[]?[,,]? TerrainData { get; set; }

        public Core.Terrain FromSchema(params object[] context)
        {
            Context worldContext = (Context)context[0];

            if (this.TerrainData == null)
            {
                throw new InvalidOperationException("Terrain data must be set to deserialize");
            }

            return new Core.Terrain(this.TerrainData, worldContext);
        }
    }
}