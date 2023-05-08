using Newtonsoft.Json;

namespace Core
{
    // A character with no preset cells.
    public class Dummy : Building
    {
        public override CharacterType Type => CharacterType.Dummy;

        protected override void InitComponents() { }

        public override Schema.Character ToSchema()
        {
            var dummy = new Schema.Dummy();
            return this.PopulateSchema(dummy);
        }

        public Dummy(Context context) : base(context) { }
    }
}