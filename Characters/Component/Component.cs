using System;
using System.Collections.Generic;
using System.Reflection;
using Core;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;

namespace Core
{
    [JsonConverter(typeof(CellConverter))]
    [JsonObject(MemberSerialization.OptIn)]
    public abstract class Component
    {
        [JsonProperty("type")]
        [JsonConverter(typeof(StringEnumConverter))]
        public abstract ComponentType Type { get; }

        public Entity Owner { get; set; }
        public virtual void Tick(float deltaTime) { }
        public virtual void OnAddToGrid() { }
        public virtual void OnRemoveFromGrid() { }
        protected World World => Owner.Context.World;

        public Component(Entity owner)
        {
            this.Owner = owner;
        }
    }

    public class CellConverter : JsonConverter
    {
        private static readonly Dictionary<ComponentType, Type> TypeMap = new Dictionary<ComponentType, Type>
        {
            { ComponentType.Conveyor, typeof(ConveyorComponent) },
            { ComponentType.Harvest, typeof(HarvestComponent) },
            { ComponentType.Inventory, typeof(InventoryComponent) },
            { ComponentType.Harvestable, typeof(HarvestableComponent) },
        };

        public override bool CanConvert(Type objectType)
        {
            return typeof(Component).IsAssignableFrom(objectType);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            var jsonObject = JObject.Load(reader);

            var typeString = jsonObject.GetValue("type", StringComparison.OrdinalIgnoreCase)?.Value<string>();
            if (!Enum.TryParse<ComponentType>(typeString, true, out ComponentType cellType))
            {
                throw new JsonSerializationException($"Invalid cell type: {typeString}");
            }

            if (!TypeMap.TryGetValue(cellType, out var targetType))
            {
                throw new InvalidOperationException($"Invalid type value '{cellType}'");
            }

            Entity? owner = serializer.Context.Context as Entity;

            if (owner == null)
            {
                throw new InvalidOperationException("Cell owner was not passed through serializer");
            }

            object? target = Activator.CreateInstance(targetType, owner);

            if (target == null)
            {
                throw new InvalidOperationException($"Failed to create instance of type '{targetType}'");
            }

            serializer.Populate(jsonObject.CreateReader(), target);

            if (!(target is Component))
            {
                throw new InvalidOperationException($"Created instance of type '{targetType}' is not a cell");
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