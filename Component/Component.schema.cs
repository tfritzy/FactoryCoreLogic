using System;
using System.Collections.Generic;
using Core;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Schema
{
    [JsonConverter(typeof(ComponentConverter))]
    public abstract class Component : SchemaOf<Core.Component>
    {
        [JsonProperty("type")]
        public abstract ComponentType Type { get; }

        public abstract Core.Component FromSchema(params object[] context);
    }

    public class ComponentConverter : JsonConverter
    {
        private static readonly Dictionary<ComponentType, Type> TypeMap = new Dictionary<ComponentType, Type>
        {
            { ComponentType.Conveyor, typeof(ConveyorComponent) },
            { ComponentType.Inventory, typeof(Inventory) },
            { ComponentType.WornItems, typeof(WornItems) },
            { ComponentType.ActiveItems, typeof(ActiveItems) },
            { ComponentType.Attack, typeof(Attack) },
            { ComponentType.Life, typeof(Life) },
            { ComponentType.TowerTargeting, typeof(TowerTargeting) },
            { ComponentType.Mine, typeof(Mine) },
            { ComponentType.TransferToConveyor, typeof(TransferToConveyor) },
            { ComponentType.TransferToInventory, typeof(TransferToInventory) },
        };

        public override bool CanConvert(Type objectType)
        {
            return typeof(Component).IsAssignableFrom(objectType);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            var jsonObject = JObject.Load(reader);

            var typeString = jsonObject.GetValue("type", StringComparison.OrdinalIgnoreCase)?.Value<string>();
            if (!Enum.TryParse<ComponentType>(typeString, true, out ComponentType componentType))
            {
                throw new JsonSerializationException($"Invalid component type: {typeString}");
            }

            if (!TypeMap.TryGetValue(componentType, out var targetType))
            {
                throw new InvalidOperationException($"Invalid type value '{componentType}'");
            }

            object? target = Activator.CreateInstance(targetType);

            if (target == null)
            {
                throw new InvalidOperationException($"Failed to create instance of type '{targetType}'");
            }

            serializer.Populate(jsonObject.CreateReader(), target);

            if (!(target is Component))
            {
                throw new InvalidOperationException($"Created instance of type '{targetType}' is not a schema.component");
            }

            return (Component)target;
        }

        public override bool CanWrite => false;

        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
