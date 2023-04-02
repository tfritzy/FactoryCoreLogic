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
            throw new System.NotImplementedException();
        }

        public Dummy(Context context) : base(context) { }
    }
}