namespace Core
{
    public class Bedrock : Hex
    {
        public override HexType Type => HexType.Bedrock;
        public override bool Indestructible => true;

        public Bedrock(Point3Int gridPosition, Context context) : base(gridPosition, context) { }

        public override Schema.Entity BuildSchemaObject()
        {
            return new Schema.Bedrock();
        }
    }
}