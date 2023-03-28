using Newtonsoft.Json;

namespace FactoryCore
{
    public class DummyBuilding : Building
    {
        // A character with no preset cells.
        public override CharacterType Type => CharacterType.Dummy;

        protected override void InitCells() { }

        public DummyBuilding(Context context) : base(context) { }
    }
}