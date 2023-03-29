using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace FactoryCore
{
    [JsonConverter(typeof(CharacterConverter))]
    [JsonObject(MemberSerialization.OptIn)]
    public abstract class Character : Entity
    {
        [JsonProperty("type")]
        public abstract CharacterType Type { get; }

        [JsonProperty("gridPosition")]
        public Point2Int GridPosition { get; protected set; }

        public Character(Context context) : base(context)
        {
        }

        public virtual void Tick(float deltaTime)
        {
            foreach (var cell in Components.Values)
            {
                cell.Tick(deltaTime);
            }
        }

        public void SetGridPosition(Point2Int gridPosition)
        {
            this.GridPosition = gridPosition;
        }

        public virtual void OnAddToGrid(Point2Int gridPosition)
        {
            this.GridPosition = gridPosition;
            foreach (var cell in Components.Values)
            {
                cell.OnAddToGrid();
            }
        }

        public virtual void OnRemoveFromGrid()
        {
            foreach (var cell in Components.Values)
            {
                cell.OnRemoveFromGrid();
            }
        }

        public void UpdateOwnerOfCells()
        {
            foreach (var cell in Components.Values)
            {
                cell.Owner = this;
            }
        }
    }

    public class CharacterConverter : JsonConverter
    {
        private static readonly Dictionary<CharacterType, Type> TypeMap = new Dictionary<CharacterType, Type>
        {
            { CharacterType.Conveyor, typeof(Conveyor) },
            { CharacterType.Dummy, typeof(DummyBuilding) },
            { CharacterType.Tree, typeof(Tree) },
        };

        public override bool CanConvert(Type objectType)
        {
            return typeof(Character).IsAssignableFrom(objectType);
        }

        public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            var jsonObject = JObject.Load(reader);

            var typeString = jsonObject.GetValue("type", StringComparison.OrdinalIgnoreCase)?.Value<string>();
            if (!Enum.TryParse<CharacterType>(typeString, true, out CharacterType characterType))
            {
                throw new JsonSerializationException($"Invalid cell type: {typeString}");
            }

            if (!TypeMap.TryGetValue(characterType, out var targetType))
            {
                throw new InvalidOperationException($"Invalid character type '{characterType}'");
            }

            Context? context = serializer.Context.Context as Context;

            if (context == null)
            {
                throw new InvalidOperationException("Context was not passed through serializer");
            }

            object? target = Activator.CreateInstance(targetType, context);

            if (target == null)
            {
                throw new InvalidOperationException($"Failed to create instance of type '{targetType}'");
            }

            StreamingContext streamingContext = new StreamingContext(StreamingContextStates.All, target);
            JsonSerializer characterSerializer = new JsonSerializer();
            characterSerializer.Context = streamingContext;
            characterSerializer.Populate(jsonObject.CreateReader(), target);

            if (!(target is Character))
            {
                throw new InvalidOperationException($"Created instance of type '{targetType}' is not a character");
            }

            Character character = (Character)target;
            character.UpdateOwnerOfCells();

            return character;
        }

        public override bool CanWrite => false;

        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            throw new NotImplementedException(); // We never need to write.
        }
    }
}