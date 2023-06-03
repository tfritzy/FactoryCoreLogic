using System;
using System.Collections.Generic;
using Core;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Schema
{
    [JsonConverter(typeof(HexConverter))]
    public abstract class Hex : Entity
    {
        [JsonProperty("type")]
        public abstract HexType Type { get; }

        [JsonProperty("pos")]
        public Point3Int GridPosition { get; set; }

        [JsonProperty("vegetation")]
        public List<Vegetation>? Vegetation { get; set; }

        protected override Core.Entity BuildCoreObject(Context context)
        {
            return Core.Hex.Create(this.Type, this.GridPosition, context);
        }

        protected override Core.Entity CreateCore(params object[] context)
        {
            if (context.Length == 0 || !(context[0] is Core.Context))
                throw new InvalidOperationException("Context is missing.");

            Core.Hex hex = (Core.Hex)base.CreateCore(context);
            if (this.Vegetation != null)
            {
                foreach (var vegetation in this.Vegetation)
                {
                    Core.Entity coreVegetation = vegetation.FromSchema(context);
                    hex.AddVegetation((Core.Vegetation)coreVegetation);
                }
            }

            return hex;
        }

        public Core.Entity FromSchema(params object[] context)
        {
            return this.CreateCore(context);
        }
    }

    public class HexConverter : JsonConverter
    {
        private static readonly Dictionary<HexType, Type> TypeMap = new Dictionary<HexType, Type>
        {
            { HexType.Dirt, typeof(DirtHex) },
            { HexType.Stone, typeof(StoneHex) },
            { HexType.Water, typeof(WaterHex) },
            { HexType.StoneStairs, typeof(StoneStairs) },
            { HexType.Bedrock, typeof(Bedrock) },
        };

        public override bool CanConvert(Type objectType)
        {
            return typeof(Component).IsAssignableFrom(objectType);
        }

        public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
            {
                return null;
            }

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
