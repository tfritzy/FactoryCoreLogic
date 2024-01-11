using System.Collections.Generic;
using Core;
using Newtonsoft.Json;

namespace Schema
{
    public class World
    {
        [JsonProperty("terrain")]
        public byte[]? Terrain;

        [JsonProperty("buildings")]
        public Dictionary<Point2Int, ulong>? Buildings;

        [JsonProperty("characters")]
        public byte[]? Characters;

        [JsonProperty("items")]
        public ItemObject[]? ItemObjects;

        public Core.World FromSchema(params object[] context)
        {
            if (Terrain == null)
                throw new System.ArgumentException("Terrain cannot be null to deserialize a World");

            Core.World world = new Core.World();
            Context worldContext = new Context(world);
            Schema.Terrain terrain = Schema.Terrain.Parser.ParseFrom(Terrain);
            world.SetTerrain(new Core.Terrain(terrain, worldContext));

            if (Characters != null)
            {
                CharacterArray characters = Schema.CharacterArray.Parser.ParseFrom(Characters);
                foreach (OneofCharacter schemaCharacter in characters.Characters)
                {
                    Core.Character character = Core.Character.FromSchema(worldContext, schemaCharacter);
                    world.AddCharacter(character);
                }
            }

            if (Buildings != null)
            {
                foreach (var kvp in Buildings)
                {
                    Core.Building? building = (Core.Building?)world.GetCharacter(kvp.Value);

                    if (building == null)
                    {
                        continue;
                    }

                    world.AddBuilding(building, (Point2Int)building.GridPosition);
                }
            }

            if (ItemObjects != null)
            {
                foreach (ItemObject schemaItemObject in ItemObjects)
                {
                    Core.ItemObject itemObject = Core.ItemObject.FromSchema(schemaItemObject);
                    world.AddItemObject(itemObject.Item, itemObject.Position, itemObject.Rotation);
                }
            }

            return world;
        }
    }
}