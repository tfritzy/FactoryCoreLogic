using Schema;

namespace Core
{
    public class TerrainObject : ISchema<SchemaTerrainObject>
    {
        public TerrainObjectType Type;

        public TerrainObject(TerrainObjectType type)
        {
            Type = type;
        }

        public SchemaTerrainObject ToSchema()
        {
            return new SchemaTerrainObject
            {
                Type = Type,
            };
        }

        public static TerrainObject FromSchema(SchemaTerrainObject schema)
        {
            return new TerrainObject(schema.Type);
        }
    }
}