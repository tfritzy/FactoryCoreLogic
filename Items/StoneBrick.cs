namespace Core
{
    public class LimestoneBrick : Item
    {
        public override ItemType Type => ItemType.LimestoneBrick;
        public override uint MaxStack => 32;
        public LimestoneBrick(ulong quantity) : base(quantity) { }
        public override float Width => .4f;
        private const string name = "Limestone brick";
        public override string Name => name;
        public override string? ChemicalFormula => "CaCO₃";
        public override Item.PlacedTriangleMetadata[]? Places => places;
        private static Item.PlacedTriangleMetadata[] places = new Item.PlacedTriangleMetadata[]
        {
            new Item.PlacedTriangleMetadata
            {
                Triangle = new Triangle(TriangleType.Stone, TriangleSubType.FullBrick),
                PositionOffset = new HexSide[] {},
                RotationOffset = HexSide.NorthEast,
            }
        };
    }
}