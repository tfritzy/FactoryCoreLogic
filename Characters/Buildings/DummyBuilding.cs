using Newtonsoft.Json;

namespace Core
{
    // A building with no preset components. Used for testing.
    public class DummyBuilding : Building
    {
        public override CharacterType Type => CharacterType.DummyBuilding;
        public override int Height => 2;

        protected override void InitComponents() { }

        public override Schema.Entity BuildSchemaObject()
        {
            return new Schema.DummyBuilding();
        }

        public DummyBuilding(Context context, int alliance) : base(context, alliance) { }
    }
}