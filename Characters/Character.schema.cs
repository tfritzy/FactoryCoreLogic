using System;
using System.Collections.Generic;
using Core;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Schema
{
    [JsonConverter(typeof(CharacterConverter))]
    public abstract class Character : Entity, SchemaOf<Core.Character>
    {
        [JsonProperty("type")]
        public abstract CharacterType Type { get; }

        [JsonProperty("gridPosition")]
        public Point2Int GridPosition { get; set; }

        [JsonProperty("cntdBy")]
        public Point3Int? ContainedByGridPosition { get; set; }

        protected Core.Character ToCore(params object[] context)
        {
            if (context.Length == 0 || context[0] == null || !(context[0] is Context))
                throw new ArgumentException("Conveyor requires a Context as context[0]");

            Context worldContext = (Context)context[0];

            var character = Core.Character.Create(this.Type, worldContext);
            character.Id = this.Id;
            character.SetGridPosition(this.GridPosition);
            character.ContainedByGridPosition = this.ContainedByGridPosition;

            if (this.Components == null)
                this.Components = new Dictionary<Core.ComponentType, Component>();

            foreach (var componentType in this.Components.Keys)
            {
                Core.Component component = this.Components[componentType].FromSchema(character);
                character.SetComponent(component);
            }

            return character;
        }

        public abstract Core.Character FromSchema(params object[] context);
    }

    public class CharacterConverter : JsonConverter
    {
        private static readonly Dictionary<CharacterType, Type> TypeMap = new Dictionary<CharacterType, Type>
        {
            { CharacterType.Conveyor, typeof(Conveyor) },
            { CharacterType.Tree, typeof(Tree) },
            { CharacterType.Dummy, typeof(Dummy) },
            { CharacterType.DummyBuilding, typeof(DummyBuilding) },
            { CharacterType.Player, typeof(Player) },
            { CharacterType.Quarry, typeof(Quarry) },
            { CharacterType.Villager, typeof(Villager) },
        };

        public override bool CanConvert(Type objectType)
        {
            return typeof(Component).IsAssignableFrom(objectType);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            var jsonObject = JObject.Load(reader);

            var typeString = jsonObject.GetValue("type", StringComparison.OrdinalIgnoreCase)?.Value<string>();
            if (!Enum.TryParse<CharacterType>(typeString, true, out CharacterType CharacterType))
            {
                throw new JsonSerializationException($"Invalid component type: {typeString}");
            }

            if (!TypeMap.TryGetValue(CharacterType, out var targetType))
            {
                throw new InvalidOperationException($"Didn't add '{CharacterType}' type to dictionary");
            }

            object? target = Activator.CreateInstance(targetType);

            if (target == null)
            {
                throw new InvalidOperationException($"Failed to create instance of type '{targetType}'");
            }

            serializer.Populate(jsonObject.CreateReader(), target);

            if (!(target is Character))
            {
                throw new InvalidOperationException($"Created instance of type '{targetType}' is not a schema.Character");
            }

            return (Character)target;
        }

        public override bool CanWrite => false;

        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
