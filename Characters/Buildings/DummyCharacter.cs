using Newtonsoft.Json;

namespace FactoryCore
{
    public class DummyBuilding : Building
    {
        // A character with no preset cells.
        public override CharacterType Type => CharacterType.Dummy;
        protected override void InitCells() { }

        [JsonConstructor]
        protected DummyBuilding() : base(null!) { }

        public DummyBuilding(World world) : base(world) { }
    }
}