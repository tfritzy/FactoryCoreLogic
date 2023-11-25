namespace Core
{
    public class DummyMob : Mob
    {
        public override CharacterType Type => CharacterType.DummyMob;
        private static readonly string name = "Dummy Mob";
        public override string Name => name;

        public override Schema.Entity BuildSchemaObject()
        {
            return new Schema.DummyMob();
        }

        public DummyMob(Context context, int alliance) : base(context, alliance)
        {
        }
    }
}