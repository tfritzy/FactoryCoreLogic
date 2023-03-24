using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace FactoryCore
{
    [JsonConverter(typeof(ItemConverter))]
    public abstract class Item
    {
        [JsonProperty("type")]
        public abstract ItemType Type { get; }

        [JsonProperty("quantity")]
        public int Quantity { get; private set; }

        public virtual float Width => 0.1f;
        public virtual int MaxStack => 1;

        public Item() : this(1) { }

        public Item(int quantity)
        {
            this.Quantity = quantity;
        }

        public void AddToStack(int amount)
        {
            if (Quantity + amount > MaxStack)
                throw new InvalidOperationException("Cannot add to stack, would exceed max stack size.");

            Quantity += amount;
        }

        public void RemoveFromStack(int amount)
        {
            if (Quantity - amount < 0)
                throw new InvalidOperationException("Cannot remove from stack, would go below 0.");

            Quantity -= amount;
        }

        public static Item? Create(ItemType? type, int quantity)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            switch (type)
            {
                case ItemType.Dirt:
                    return new Dirt(quantity);
                case ItemType.Stone:
                    return new Stone(quantity);
                case ItemType.Wood:
                    return new Wood(quantity);
                default:
                    throw new ArgumentException("Invalid item type " + type);
            }
        }
    }


    public class ItemConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Item);
        }

        public override Item? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
                throw new InvalidOperationException("Cannot read null item.");

            JObject obj = JObject.Load(reader);
            ItemType? type = obj["type"]?.ToObject<ItemType>();
            int quantity = obj["quantity"]?.ToObject<int>() ?? 1;
            return Item.Create(type, quantity);
        }

        public override bool CanWrite => false;

        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            throw new NotImplementedException(); // We never need to write.
        }
    }
}