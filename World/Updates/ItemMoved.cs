using System.Text;
using Newtonsoft.Json;

namespace Core
{
    public class ItemMoved : Update
    {
        [JsonProperty("type")]
        public override UpdateType Type => UpdateType.ItemMoved;

        [JsonProperty("id")]
        public ulong Id;

        [JsonProperty("pos")]
        public Point3Float Pos;

        [JsonProperty("rotation")]
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