using Newtonsoft.Json;

namespace Core
{
    public class Dummy : Building
    {
        // A character with no preset cells.
        public override CharacterType Type => CharacterType.Dummy;

        protected override void InitComponents() { }

        public Dummy(Context context) : base(context) { }
    }
}