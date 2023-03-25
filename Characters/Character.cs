using System;
using System.Collections.Generic;
using FactoryCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace FactoryCore
{
    [JsonConverter(typeof(CharacterConverter))]
    [JsonObject(MemberSerialization.OptIn)]
    public abstract class Character
    {
        [JsonProperty("type")]
        public abstract CharacterType Type { get; }

        [JsonProperty("gridPosition")]
        public Point2Int GridPosition { get; protected set; }

        [JsonProperty("cells")]
        protected Dictionary<Type, Cell> Cells;

        public World? World { get; set; }
        public InventoryCell? Inventory => GetCell<InventoryCell>();
        public ConveyorCell? Conveyor => GetCell<ConveyorCell>();

        protected virtual void InitCells() { }

        public Character(World? world)
        {
            this.World = world;
            this.Cells = new Dictionary<Type, Cell>();
            InitCells();
        }

        public virtual void Tick(float deltaTime)
        {
            foreach (var cell in Cells.Values)
            {
                cell.Tick(deltaTime);
            }
        }

        public T GetCell<T>() where T : Cell
        {
            if (!Cells.ContainsKey(typeof(T)))
            {
                return default(T)!;
            }

            return (T)Cells[typeof(T)];
        }

        public void SetCell(Cell cell)
        {
            Cells[cell.GetType()] = cell;
        }

        public void SetGridPosition(Point2Int gridPosition)
        {
            this.GridPosition = gridPosition;
        }

        public virtual void OnAddToGrid(Point2Int gridPosition)
        {
            this.GridPosition = gridPosition;
            foreach (var cell in Cells.Values)
            {
                cell.OnAddToGrid();
            }
        }

        public virtual void OnRemoveFromGrid()
        {
            foreach (var cell in Cells.Values)
            {
                cell.OnRemoveFromGrid();
            }
        }
    }

    public class CharacterConverter : JsonConverter
    {
        private static readonly Dictionary<CharacterType, Type> TypeMap = new Dictionary<CharacterType, Type>
        {
            { CharacterType.Conveyor, typeof(Conveyor) },
            { CharacterType.Dummy, typeof(DummyCharacter) },
        };

        public override bool CanConvert(Type objectType)
        {
            return typeof(Character).IsAssignableFrom(objectType);
        }

        public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            var jsonObject = JObject.Load(reader);

            var typeString = jsonObject.GetValue("type", StringComparison.OrdinalIgnoreCase)?.Value<string>();
            if (!Enum.TryParse<CharacterType>(typeString, true, out CharacterType cellType))
            {
                throw new JsonSerializationException($"Invalid cell type: {typeString}");
            }

            if (!TypeMap.TryGetValue(cellType, out var targetType))
            {
                throw new InvalidOperationException($"Invalid type value '{cellType}'");
            }

            object? target = Activator.CreateInstance(targetType);

            if (target == null)
            {
                throw new InvalidOperationException($"Failed to create instance of type '{targetType}'");
            }

            serializer.Populate(jsonObject.CreateReader(), target);

            if (!(target is Character))
            {
                throw new InvalidOperationException($"Created instance of type '{targetType}' is not a character");
            }

            return (Character)target;
        }

        public override bool CanWrite => false;

        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            throw new NotImplementedException(); // We never need to write.
        }
    }
}