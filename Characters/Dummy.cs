using Newtonsoft.Json;

namespace Core
{
    public class Dummy : Unit
    {
        public override CharacterType Type => CharacterType.Dummy;

        protected override void InitComponents() { }

        public override Schema.Entity BuildSchemaObject()
        {
            return new Schema.Dummy();
        }

        public Dummy(Context context, int alliance) : base(context, alliance) { }
    }
}