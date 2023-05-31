using Newtonsoft.Json;

namespace Core
{
    public class Dummy : Unit
    {
        public override CharacterType Type => CharacterType.Dummy;

        protected override void InitComponents() { }

        public override Schema.Character ToSchema()
        {
            var dummy = new Schema.Dummy();
            return this.PopulateSchema(dummy);
        }

        public Dummy(Context context, int alliance) : base(context, alliance) { }
    }
}