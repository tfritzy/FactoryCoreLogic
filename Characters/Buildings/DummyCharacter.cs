namespace FactoryCore
{
    public class DummyCharacter : Character
    {
        // A character with no preset cells.
        public DummyCharacter(World world) : base(world) { }
        public override CharacterType Type => CharacterType.Dummy;
        protected override void InitCells() { }
    }
}