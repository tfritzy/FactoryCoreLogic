namespace Core
{
    public class Bedrock : Hex
    {
        public override HexType Type => HexType.Bedrock;

        public Bedrock(Point3Int gridPosition, Context context) : base(gridPosition, context) { }

        public override Schema.Hex BuildSchemaObject()
        {
            return new Schema.Bedrock();
        }
    }
}