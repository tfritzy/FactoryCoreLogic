using Newtonsoft.Json;

namespace Core
{
    // A building with no preset components. Used for testing.
    public class DummyBuilding : Building
    {
        public override CharacterType Type => CharacterType.DummyBuilding;

        protected override void InitComponents() { }

        public override Schema.Character ToSchema()
        {
            var dummy = new Schema.DummyBuilding();
            return this.PopulateSchema(dummy);
        }

        public DummyBuilding(Context context) : base(context) { }
    }
}