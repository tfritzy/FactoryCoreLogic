using System;
using System.Collections.Generic;
using Core;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Schema
{
    [JsonConverter(typeof(HexConverter))]
    public abstract class Hex : Entity, Schema<Core.Hex>
    {
        [JsonProperty("type")]
        public abstract HexType Type { get; }

        [JsonProperty("pos")]
        public Point3Int? GridPosition { get; set; }

        [JsonProperty("entities")]
        public List<ulong>? ContainedEntities { get; set; }

        public abstract Core.Hex FromSchema(params object[] context);
    }

    public class HexConverter : JsonConverter
    {
        private static readonly Dictionary<HexType, Type> TypeMap = new Dictionary<HexType, Type>
        {
            { HexType.Dirt, typeof(DirtHex) },
            { HexType.Stone, typeof(StoneHex) },
        };

        public override bool CanConvert(Type objectType)
        {
            return typeof(Component).IsAssignableFrom(objectType);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            var jsonObject = JObject.Load(reader);

            var typeString = jsonObject.GetValue("type", StringComparison.OrdinalIgnoreCase)?.Value<string>();
            if (!Enum.TryParse<HexType>(typeString, true, out HexType HexType))
            {
                throw new JsonSerializationException($"Invalid component type: {typeString}");
            }

            if (!TypeMap.TryGetValue(HexType, out var targetType))
            {
                throw new InvalidOperationException($"Didn't add '{HexType}' type to dictionary");
            }

            object? target = Activator.CreateInstance(targetType);

            if (target == null)
            {
                throw new InvalidOperationException($"Failed to create instance of type '{targetType}'");
            }

            serializer.Populate(jsonObject.CreateReader(), target);

            if (!(target is Hex))
            {
                throw new InvalidOperationException($"Created instance of type '{targetType}' is not a schema.Hex");
            }

            return (Hex)target;
        }

        public override bool CanWrite => false;

        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
