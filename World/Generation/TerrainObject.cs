using Schema;

namespace Core
{
    public class TerrainObject : ISchema<SchemaTerrainObject>
    {
        public TerrainObjectType Type;
        public float PercentHarvested;

        public SchemaTerrainObject ToSchema()
        {
            return new SchemaTerrainObject
            {
                Type = Type,
                PercentHarvested = PercentHarvested,
            };
        }

        public static TerrainObject FromSchema(SchemaTerrainObject schema)
        {
            return new TerrainObject
            {
                Type = schema.Type,
                PercentHarvested = schema.PercentHarvested,
            };
        }
    }
}