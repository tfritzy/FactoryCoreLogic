using System.Collections.Generic;
using Core;
using Newtonsoft.Json;

namespace Schema
{
    public class Terrain : SchemaOf<Core.Terrain>
    {
        [JsonProperty("terrain")]
        public TerrainPoint?[,,]? TerrainData { get; set; }

        public Core.Terrain FromSchema(params object[] context)
        {
            return new Core.Terrain(TerrainData!);
        }
    }
}