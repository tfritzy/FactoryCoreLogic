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

        [JsonProperty("alliance")]
        public int Alliance { get; set; }

        [JsonProperty("pos")]
        public Point3Int GridPosition { get; set; }

        protected override Core.Entity BuildCoreObject(Context context)
        {
            var character = Core.Character.Create(this.Type, context, Alliance);
            character.GridPosition = this.GridPosition;
            return character;
        }

        public Core.Character FromSchema(params object[] context)
        {
            return (Core.Character)this.CreateCore(context);
        }
    }

    public class CharacterConverter : JsonConverter
    {
        private static readonly Dictionary<CharacterType, Type> TypeMap = new Dictionary<CharacterType, Type>
        {
            { CharacterType.Conveyor, typeof(Conveyor) },
            { CharacterType.Dummy, typeof(Dummy) },
            { CharacterType.DummyBuilding, typeof(DummyBuilding) },
            { CharacterType.Player, typeof(Player) },
            { CharacterType.GuardTower, typeof(GuardTower) },
            { CharacterType.Pikeman, typeof (Pikeman) },
            { CharacterType.DummyMob, typeof(DummyMob) },
            { CharacterType.Keep, typeof (Keep) },
            { CharacterType.Mineshaft, typeof (Mineshaft) },
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
