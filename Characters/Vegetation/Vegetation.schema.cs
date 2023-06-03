using System;
using System.Collections.Generic;
using Core;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Schema
{
    [JsonConverter(typeof(VegetationConverter))]
    public abstract class Vegetation : Entity
    {
        [JsonProperty("type")]
        public abstract VegetationType Type { get; }

        [JsonProperty("offst")]
        public Point2Float PositionOffset { get; set; }

        protected override Core.Entity BuildCoreObject(Context context)
        {
            return Core.Vegetation.Create(this.Type, context);
        }

        protected override Core.Entity CreateCore(params object[] context)
        {
            if (context.Length == 0 || !(context[0] is Core.Context))
                throw new InvalidOperationException("Context is missing.");

            Core.Vegetation vegetation = (Core.Vegetation)base.CreateCore(context);
            vegetation.PositionOffset = this.PositionOffset;

            return vegetation;
        }

        public Core.Entity FromSchema(params object[] context)
        {
            return this.CreateCore(context);
        }
    }


    public class VegetationConverter : JsonConverter
    {
        private static readonly Dictionary<VegetationType, Type> TypeMap = new Dictionary<VegetationType, Type>
        {
            { VegetationType.Tree, typeof(Tree) },
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
            if (!Enum.TryParse<VegetationType>(typeString, true, out VegetationType vegetationType))
            {
                throw new JsonSerializationException($"Invalid component type: {typeString}");
            }

            if (!TypeMap.TryGetValue(vegetationType, out var targetType))
            {
                throw new InvalidOperationException($"Didn't add '{vegetationType}' type to dictionary");
            }

            object? target = Activator.CreateInstance(targetType);

            if (target == null)
            {
                throw new InvalidOperationException($"Failed to create instance of type '{targetType}'");
            }

            serializer.Populate(jsonObject.CreateReader(), target);

            if (!(target is Vegetation))
            {
                throw new InvalidOperationException($"Created instance of type '{targetType}' is not a schema.Vegetation");
            }

            return (Vegetation)target;
        }

        public override bool CanWrite => false;

        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}