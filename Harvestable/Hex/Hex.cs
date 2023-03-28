using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System; // Needed in 4.7.1
using System.Runtime.Serialization;

namespace FactoryCore
{
    [JsonConverter(typeof(HexConverter))]
    [JsonObject(MemberSerialization.OptIn)]
    public abstract class Hex : EntityComponent
    {
        [JsonProperty("type")]
        public abstract HexType Type { get; }

        [JsonProperty("gridPosition")]
        public Point3Int GridPosition { get; protected set; }

        protected Hex(Point3Int gridPosition, Context context) : base(context)
        {
            this.GridPosition = gridPosition;
        }

        public static Hex? Create(HexType? type, Point3Int gridPosition, Context context)
        {
            if (type == null)
                return null;

            switch (type)
            {
                case HexType.Dirt:
                    return new DirtHex(gridPosition, context);
                case HexType.Stone:
                    return new StoneHex(gridPosition, context);
                default:
                    throw new ArgumentException("Invalid hex type " + type);
            }
        }
    }

    public class HexConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Hex);
        }

        public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
                return null;

            Context? context = serializer.Context.Context as Context;

            if (context == null)
                throw new InvalidOperationException("Context was not availble in the deserializer.");

            JObject obj = JObject.Load(reader);
            HexType? type = obj["type"]?.ToObject<HexType>();
            Point3Int? gridPosition = obj["gridPosition"]?.ToObject<Point3Int>();

            if (gridPosition == null)
                throw new InvalidOperationException("Grid position was not available on a hex.");

            Hex? hex = Hex.Create(type, gridPosition.Value, context);

            if (hex == null)
                throw new InvalidOperationException("Hex was not available.");

            StreamingContext streamingContext = new StreamingContext(StreamingContextStates.All, hex);
            JsonSerializer hexSerializer = new JsonSerializer();
            hexSerializer.Context = streamingContext;
            hexSerializer.Populate(obj.CreateReader(), hex);

            return hex;
        }

        public override bool CanWrite => false;

        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            throw new NotImplementedException(); // We never need to write.
        }
    }
}