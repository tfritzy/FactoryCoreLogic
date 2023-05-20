namespace Core
{
    public class StoneStairs : Hex
    {
        public override HexType Type => HexType.StoneStairs;

        public StoneStairs(Point3Int gridPosition, Context context) : base(gridPosition, context) { }

        public override Schema.Hex BuildSchemaObject()
        {
            return new Schema.StoneStairs();
        }
    }
}