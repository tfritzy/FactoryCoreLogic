namespace Core
{
    public class DummyMob : Mob
    {
        public override CharacterType Type => CharacterType.DummyMob;
        private static readonly string name = "Dummy Mob";
        public override string Name => name;

        public new Schema.OneofCharacter ToSchema()
        {
            return new Schema.OneofCharacter
            {
                DummyMob = new Schema.DummyMob()
                {
                    Character = base.ToSchema(),
                }
            };
        }

        public DummyMob(Context context, int alliance) : base(context, alliance)
        {
        }
    }
}