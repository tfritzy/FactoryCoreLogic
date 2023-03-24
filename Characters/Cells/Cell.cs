using System;
using System.Collections.Generic;
using FactoryCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;

namespace FactoryCore
{
    [JsonConverter(typeof(CellConverter))]
    [JsonObject(MemberSerialization.OptIn)]
    public abstract class Cell
    {
        [JsonProperty("type")]
        [JsonConverter(typeof(StringEnumConverter))]
        public abstract CellType Type { get; }

        public Character Owner { get; private set; }
        public virtual void Tick(float deltaTime) { }
        public virtual void OnAddToGrid() { }
        public virtual void OnRemoveFromGrid() { }
        protected World World => Owner.World;

        public Cell(Character owner)
        {
            this.Owner = owner;
        }
    }

    public class CellConverter : JsonConverter
    {
        private static readonly Dictionary<CellType, Type> TypeMap = new Dictionary<CellType, Type>
    {
        { CellType.Conveyor, typeof(ConveyorCell) },
        { CellType.Harvest, typeof(HarvestCell) },
        { CellType.Inventory, typeof(InventoryCell) },
    };

        public override bool CanConvert(Type objectType)
        {
            return typeof(Cell).IsAssignableFrom(objectType);
        }

        public override Cell? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            var jsonObject = JObject.Load(reader);

            var typeString = jsonObject.GetValue("type", StringComparison.OrdinalIgnoreCase)?.Value<string>();
            if (!Enum.TryParse<CellType>(typeString, true, out CellType cellType))
            {
                throw new JsonSerializationException($"Invalid cell type: {typeString}");
            }

            if (!TypeMap.TryGetValue(cellType, out var targetType))
            {
                throw new InvalidOperationException($"Invalid type value '{cellType}'");
            }

            Character owner = (Character)serializer.Context.Context;

            if (owner == null)
            {
                throw new InvalidOperationException($"Owner was not passed through the context");
            }

            object? target = Activator.CreateInstance(targetType, owner);

            if (target == null)
            {
                throw new InvalidOperationException($"Failed to create instance of type '{targetType}'");
            }

            serializer.Populate(jsonObject.CreateReader(), target);

            if (!(target is Cell))
            {
                throw new InvalidOperationException($"Created instance of type '{targetType}' is not a cell");
            }

            return (Cell)target;
        }

        public override bool CanWrite => false;

        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}