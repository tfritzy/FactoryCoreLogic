using System;
using System.Collections.Generic;

namespace Core
{
    public abstract class Item
    {
        public abstract ItemType Type { get; }
        public int Quantity { get; private set; }
        public ulong Id { get; set; }

        public virtual float Width => 0.1f;
        public virtual int MaxStack => 1;
        public virtual Dictionary<ItemType, int>? Recipe => null;
        public virtual CharacterType? Builds => null;

        public Item() : this(1) { }

        public Item(int quantity)
        {
            this.Quantity = quantity;
            this.Id = IdGenerator.GenerateId();
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

            if (amount < 0)
                throw new InvalidOperationException("Cannot remove negative amount from stack.");

            Quantity -= amount;
        }

        public void SetQuantity(int quantity)
        {
            if (quantity > MaxStack)
                throw new InvalidOperationException("Cannot set quantity, would exceed max stack size.");

            Quantity = quantity;
        }

        public static Item Create(ItemType type)
        {
            switch (type)
            {
                case ItemType.Dirt:
                    return new Dirt();
                case ItemType.Stone:
                    return new Stone();
                case ItemType.Wood:
                    return new Wood();
                case ItemType.Arrowhead:
                    return new Arrowhead();
                case ItemType.ToolShaft:
                    return new ToolShaft();
                case ItemType.Log:
                    return new Log();
                case ItemType.IronBar:
                    return new IronBar();
                case ItemType.IronPickaxe:
                    return new IronPickaxe();
                case ItemType.Conveyor:
                    return new ConveyorItem();
                case ItemType.Coal:
                    return new Coal();
                case ItemType.IronOre:
                    return new IronOre();
                case ItemType.Mineshaft:
                    return new MineshaftItem();
                case ItemType.Depot:
                    return new DepotItem();
                default:
                    throw new ArgumentException("Invalid item type " + type);
            }
        }

        public Schema.Item ToSchema()
        {
            return new Schema.Item()
            {
                Type = Type,
                Quantity = Quantity,
                Id = Id,
            };
        }

        public override bool Equals(object? obj)
        {
            if (obj is Item item)
            {
                return item.Type == Type && item.Quantity == Quantity;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Type, Quantity);
        }
    }
}