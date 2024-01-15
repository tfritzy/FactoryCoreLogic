using System.Text;
using System.Text.Json.Serialization;

namespace Core
{
    public class ItemMoved : Update
    {
        [JsonPropertyName("type")]
        public override UpdateType Type => UpdateType.ItemMoved;

        [JsonPropertyName("id")]
        public ulong Id;

        [JsonPropertyName("pos")]
        public Point3Float Pos;

        [JsonPropertyName("rotation")]
        public Point3Float Rotation;

        public ItemMoved(ulong id, Point3Float pos, Point3Float rotation)
        {
            this.Pos = pos;
            this.Id = id;
            this.Rotation = rotation;
        }

        public Schema.OneofUpdate ToSchema()
        {
            return new Schema.OneofUpdate
            {
                ItemMoved = new Schema.ItemMoved
                {
                    ItemId = Id,
                    UpdatedPosition = Pos.ToSchema(),
                    Rotation = Rotation.ToSchema(),
                }
            };
        }
    }
}