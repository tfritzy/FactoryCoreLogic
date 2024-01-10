using Schema;

namespace Core
{
    public class TerrainObject
    {
        public TerrainObjectType Type;

        public TerrainObject(TerrainObjectType type)
        {
            Type = type;
        }

        public Schema.TerrainObject ToSchema()
        {
            return new Schema.TerrainObject
            {
                Type = Type,
            };
        }

        public static TerrainObject FromSchema(Schema.TerrainObject schema)
        {
            return new TerrainObject(schema.Type);
        }
    }
}