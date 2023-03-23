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

        protected Dictionary<CellType, Cell> Cells;

        public World World { get; private set; }
        public InventoryCell? Inventory => GetCell<InventoryCell>(CellType.Inventory);

        protected virtual void InitCells() { }

        public Character(World world)
        {
            this.World = world;
            this.Cells = new Dictionary<CellType, Cell>();
            InitCells();
        }

        public virtual void Tick(float deltaTime)
        {
            foreach (var cell in Cells.Values)
            {
                cell.Tick(deltaTime);
            }
        }

        public T GetCell<T>(CellType type) where T : Cell
        {
            if (!Cells.ContainsKey(type))
            {
                return default(T);
            }

            return (T)Cells[type];
        }

        public void SetCell(Cell cell)
        {
            Cells[cell.Type] = cell;
        }

        public bool HasCell(CellType type)
        {
            return Cells.ContainsKey(type);
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

        public static Character Create(CharacterType? type, World world)
        {
            if (type == null)
                throw new ArgumentException("Invalid character type " + type);

            switch (type)
            {
                case CharacterType.Dummy:
                    return new DummyCharacter(world);
                case CharacterType.Conveyor:
                    return new Conveyor(world);
                default:
                    throw new ArgumentException("Invalid character type " + type);
            }
        }
    }

    public class CharacterConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Character);
        }

        public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
                return null;

            JObject obj = JObject.Load(reader);
            CharacterType? type = obj["type"]?.ToObject<CharacterType>();

            if (serializer.Context.Context == null || !(serializer.Context.Context is World))
                throw new JsonSerializationException("No world context provided to character converter.");

            Character character = Character.Create(type, (World)serializer.Context.Context);

            Point2Int? gridPosition = obj["gridPosition"]?.ToObject<Point2Int>();

            if (gridPosition == null)
                throw new JsonSerializationException("No grid position provided to character converter.");

            character.SetGridPosition(gridPosition ?? new Point2Int(0, 0));

            return character;
        }

        public override bool CanWrite => false;

        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            throw new NotImplementedException(); // We never need to write.
        }
    }
}