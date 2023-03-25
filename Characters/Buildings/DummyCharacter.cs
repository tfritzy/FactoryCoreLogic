using Newtonsoft.Json;

namespace FactoryCore
{
    public class DummyCharacter : Character
    {
        // A character with no preset cells.
        public override CharacterType Type => CharacterType.Dummy;
        protected override void InitCells() { }

        [JsonConstructor]
        public DummyCharacter() : base(null) { }

        public DummyCharacter(World world) : base(world) { }
    }
}