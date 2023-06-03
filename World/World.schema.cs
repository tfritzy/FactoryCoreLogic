using System.Collections.Generic;
using Core;
using Newtonsoft.Json;

namespace Schema
{
    public class World : SchemaOf<Core.World>
    {
        [JsonProperty("hexes")]
        public Hex?[,,]? Hexes;

        [JsonProperty("buildings")]
        public Dictionary<Point2Int, ulong>? Buildings;

        [JsonProperty("characters")]
        public Dictionary<ulong, Character>? Characters;

        private Core.Hex?[,,] HexesFromSchema(Context context)
        {
            if (Hexes == null)
                throw new System.ArgumentException("Hexes cannot be null to deserialize a World");

            Core.Hex?[,,] hexes = new Core.Hex[Hexes.GetLength(0), Hexes.GetLength(1), Hexes.GetLength(2)];
            for (int x = 0; x < Hexes.GetLength(0); x++)
            {
                for (int y = 0; y < Hexes.GetLength(1); y++)
                {
                    for (int z = 0; z < Hexes.GetLength(2); z++)
                    {
                        hexes[x, y, z] = (Core.Hex?)Hexes[x, y, z]?.FromSchema(context);
                    }
                }
            }

            return hexes;
        }

        public Core.World FromSchema(params object[] context)
        {
            if (Hexes == null)
                throw new System.ArgumentException("Hexes cannot be null to deserialize a World");

            Core.World world = new Core.World();
            Context worldContext = new Context(world);
            world.SetHexes(HexesFromSchema(worldContext));

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

                    world.AddBuilding(building, kvp.Key);
                }
            }

            return world;
        }
    }
}