using System;
using System.Collections.Generic;

namespace Core
{
    public abstract class Item
    {
        public abstract ItemType Type { get; }
        public abstract string Name { get; }
        public abstract string? ChemicalFormula { get; }
        public uint Quantity { get; private set; }
        public virtual UnitType Units => UnitType.Unit;
        public ulong Id { get; set; }

        public virtual float Width => 0.3f;
        public virtual uint MaxStack => 1;
        public virtual Dictionary<ItemType, uint>? Recipe => null;
        public virtual CharacterType? Builds => null;
        public virtual PlacedTriangleMetadata[]? Places => null;
        public virtual CombustionProperties? Combustion => null;
        public virtual float? SpecificHeat_JoulesPerMgPerDegreeCelsious => null;
        public virtual float? MeltingPoint_Celsious => null;

        public struct CombustionProperties
        {
            public int BurnRateMilligramPerSecond;
            public float CalorificValue_JoulesPerMg;
        }

        public enum UnitType
        {
            Unit,
            Milligram
        }

        public struct PlacedTriangleMetadata
        {
            public Triangle Triangle;
            public HexSide[] PositionOffset;
            public HexSide RotationOffset;
        }

        public Item(uint quantity)
        {
            this.Quantity = quantity;
            this.Id = IdGenerator.GenerateId();
        }

        public void AddToStack(uint amount)
        {
            if (Quantity + amount > MaxStack)
                throw new InvalidOperationException("Cannot add to stack, would exceed max stack size.");

            Quantity += amount;
        }

        public void RemoveFromStack(uint amount)
        {
            if (amount > Quantity)
                throw new InvalidOperationException("Cannot remove from stack, would go below 0.");

            Quantity -= amount;
        }

        public void SetQuantity(uint quantity)
        {
            if (quantity > MaxStack)
                throw new InvalidOperationException("Cannot set quantity, would exceed max stack size.");

            Quantity = quantity;
        }

        public static Item Create(ItemType type, uint quantity = 1)
        {
            switch (type)
            {
                case ItemType.Dirt:
                    return new Dirt(quantity);
                case ItemType.Limestone:
                    return new Limestone(quantity);
                case ItemType.LimestoneBrick:
                    return new LimestoneBrick(quantity);
                case ItemType.LimestoneDoubleBrick:
                    return new LimestoneDoubleBrick(quantity);
                case ItemType.Wood:
                    return new Wood(quantity);
                case ItemType.Arrowhead:
                    return new Arrowhead(quantity);
                case ItemType.ToolShaft:
                    return new ToolShaft(quantity);
                case ItemType.Log:
                    return new Log(quantity);
                case ItemType.IronBar:
                    return new IronBar(quantity);
                case ItemType.IronPickaxe:
                    return new IronPickaxe(quantity);
                case ItemType.Conveyor:
                    return new ConveyorItem(quantity);
                case ItemType.AnthraciteCoal:
                    return new AnthraciteCoal(quantity);
                case ItemType.BituminousCoal:
                    return new BituminousCoal(quantity);
                case ItemType.LigniteCoal:
                    return new LigniteCoal(quantity);
                case ItemType.Chalcopyrite:
                    return new Chalcopyrite(quantity);
                case ItemType.Mineshaft:
                    return new MineshaftItem(quantity);
                case ItemType.ClayFurnace:
                    return new ClayFurnaceItem(quantity);
                case ItemType.Sorter:
                    return new SorterItem(quantity);
                case ItemType.Depot:
                    return new DepotItem(quantity);
                case ItemType.CopperBar:
                    return new CopperBar(quantity);
                case ItemType.Magnetite:
                    return new Magnetite(quantity);
                case ItemType.IronSiliconSlag:
                    return new IronSiliconSlag(quantity);
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