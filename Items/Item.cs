using System;
using System.Collections.Generic;

namespace Core
{
    public abstract class Item
    {
        public abstract ItemType Type { get; }
        public abstract string Name { get; }
        public abstract string? ChemicalFormula { get; }
        public int Quantity { get; private set; }
        public ulong Id { get; set; }

        public virtual float Width => 0.1f;
        public virtual int MaxStack => 1;
        public virtual Dictionary<ItemType, int>? Recipe => null;
        public virtual CharacterType? Builds => null;
        public virtual PlacedTriangleMetadata[]? Places => null;
        public virtual CombustionProperties? Combustion => null;
        public virtual float? SpecificHeat_JoulesPerKgPerDegreeCelsious => null;
        public virtual float? MeltingPoint_Celsious => null;

        public struct CombustionProperties
        {
            public float BurnRateKgPerSecond;
            public float CalorificValue_JoulesPerKg;
        }

        public struct PlacedTriangleMetadata
        {
            public Triangle Triangle;
            public HexSide[] PositionOffset;
            public HexSide RotationOffset;
        }

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
                case ItemType.Limestone:
                    return new Limestone();
                case ItemType.LimestoneBrick:
                    return new LimestoneBrick();
                case ItemType.LimestoneDoubleBrick:
                    return new LimestoneDoubleBrick();
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
                case ItemType.AnthraciteCoal:
                    return new AnthraciteCoal();
                case ItemType.BituminousCoal:
                    return new BituminousCoal();
                case ItemType.LigniteCoal:
                    return new LigniteCoal();
                case ItemType.Chalcopyrite:
                    return new Chalcopyrite();
                case ItemType.Mineshaft:
                    return new MineshaftItem();
                case ItemType.Depot:
                    return new DepotItem();
                case ItemType.CopperBar:
                    return new CopperBar();
                case ItemType.Magnetite:
                    return new Magnetite();
                case ItemType.IronSiliconSlag:
                    return new IronSiliconSlag();
                default:
                    throw new ArgumentException("Invalid item type " + type);
            }
        }

        private static Dictionary<ItemType, Item>? _itemProperties = null;
        public static Dictionary<ItemType, Item> ItemProperties
        {
            get
            {
                if (_itemProperties == null)
                {
                    _itemProperties = new Dictionary<ItemType, Item>();
                    foreach (ItemType type in Enum.GetValues(typeof(ItemType)))
                    {
                        if (type == ItemType.Invalid)
                        {
                            continue;
                        }

                        _itemProperties[type] = Create(type);
                    }
                }

                return _itemProperties;
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