using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace FactoryCore
{
    [JsonConverter(typeof(HexConverter))]
    [JsonObject(MemberSerialization.OptIn)]
    public abstract class Hex : Harvestable
    {
        [JsonProperty("type")]
        public abstract HexType Type { get; }

        public static Hex? Create(HexType? type)
        {
            if (type == null)
                return null;

            switch (type)
            {
                case HexType.Dirt:
                    return new DirtHex();
                case HexType.Stone:
                    return new StoneHex();
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

            JObject obj = JObject.Load(reader);
            HexType? type = obj["type"]?.ToObject<HexType>();
            return Hex.Create(type);
        }

        public override bool CanWrite => false;

        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            throw new NotImplementedException(); // We never need to write.
        }
    }
}