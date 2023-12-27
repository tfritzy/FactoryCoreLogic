namespace Core
{
    public class LimestoneDoubleBrick : Item
    {
        public override ItemType Type => ItemType.LimestoneDoubleBrick;
        public override uint MaxStack => 32;
        public LimestoneDoubleBrick(uint quantity) : base(quantity) { }
        public override float Width => .4f;
        private const string name = "Limestone double brick";
        public override string Name => name;
        public override string? ChemicalFormula => "CaCO₃";

        public override Item.PlacedTriangleMetadata[]? Places => places;
        private static Item.PlacedTriangleMetadata[] places = new Item.PlacedTriangleMetadata[]
        {
            new Item.PlacedTriangleMetadata
            {
                Triangle = new Triangle(TriangleType.Stone, TriangleSubType.BrickHalf),
                PositionOffset = new HexSide[] {},
                RotationOffset = HexSide.NorthEast,
            },
            new Item.PlacedTriangleMetadata
            {
                Triangle = new Triangle(TriangleType.Stone, TriangleSubType.BrickHalf),
                PositionOffset = new HexSide[] { HexSide.NorthEast},
                RotationOffset = HexSide.SouthWest,
            }
        };
    }
}