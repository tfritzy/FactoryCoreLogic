using System.Collections.Generic;
using Core;
using Newtonsoft.Json;

namespace Schema
{
    public class World : SchemaOf<Core.World>
    {
        [JsonProperty("terrain")]
        public Schema.Terrain? Terrain;

        [JsonProperty("buildings")]
        public Dictionary<Point2Int, ulong>? Buildings;

        [JsonProperty("characters")]
        public Dictionary<ulong, Character>? Characters;

        public Core.World FromSchema(params object[] context)
        {
            if (Terrain == null)
                throw new System.ArgumentException("Terrain cannot be null to deserialize a World");

            Core.World world = new Core.World();
            Context worldContext = new Context(world);
            world.SetTerrain(Terrain.FromSchema(worldContext));

            if (Characters != null)
            {
                foreach (ulong characterId in Characters.Keys)
                {
                    Core.Character character = Characters[characterId].FromSchema(worldContext);

                    world.AddCharacter(character);
                }
            }

            if (Buildings != null)
            {
                foreach (var kvp in Buildings)
                {
                    Building? building = (Building?)world.GetCharacter(kvp.Value);

                    if (building == null)
                    {
                        continue;
                    }

                    world.AddBuilding(building, (Point2Int)building.GridPosition);
                }
            }

            return world;
        }
    }
}