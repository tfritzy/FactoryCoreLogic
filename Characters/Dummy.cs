using Newtonsoft.Json;

namespace Core
{
    public class Dummy : Unit
    {
        public override CharacterType Type => CharacterType.Dummy;
        private static readonly string name = "Dummy";
        public override string Name => name;

        protected override void InitComponents() { }

        public override Schema.Entity BuildSchemaObject()
        {
            return new Schema.Dummy();
        }

        public Dummy(Context context, int alliance) : base(context, alliance) { }
    }
}