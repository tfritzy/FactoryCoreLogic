namespace Core
{
    public class StoneDoubleBrick : Item
    {
        public override ItemType Type => ItemType.StoneDoubleBrick;
        public override int MaxStack => 32;
        public StoneDoubleBrick(int quantity) : base(quantity) { }
        public StoneDoubleBrick() : base() { }
        public override float Width => .4f;
        private const string name = "Stone double brick";
        public override string Name => name;
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