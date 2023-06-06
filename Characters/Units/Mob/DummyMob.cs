namespace Core
{
    public class DummyMob : Mob
    {
        public override CharacterType Type => CharacterType.DummyMob;

        public override Schema.Entity BuildSchemaObject()
        {
            return new Schema.DummyMob();
        }

        public DummyMob(Context context, int alliance) : base(context, alliance)
        {
        }
    }
}