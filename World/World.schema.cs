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

        [JsonProperty("items")]
        public Dictionary<ulong, ItemObject>? ItemObject;

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
                    Core.Building? building = (Core.Building?)world.GetCharacter(kvp.Value);

                    if (building == null)
                    {
                        continue;
                    }

                    world.AddBuilding(building, (Point2Int)building.GridPosition);
                }
            }

            if (ItemObject != null)
            {
                foreach (ulong itemId in ItemObject.Keys)
                {
                    Core.ItemObject itemObject = ItemObject[itemId].FromSchema(worldContext);
                    world.AddItemObject(itemObject.Item, itemObject.Position, itemObject.Rotation);
                }
            }

            return world;
        }
    }
}