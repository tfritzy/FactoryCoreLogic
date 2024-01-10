namespace Core
{
    public class Keep : Building
    {
        public override CharacterType Type => CharacterType.Keep;
        public static Point3Float ProjectileOffset = new Point3Float(0, 0, 3f);
        public override Point3Float ProjectileSpawnOffset => ProjectileOffset;
        public override int Height => 6;
        private static readonly string name = "Keep";
        public override string Name => name;

        public new Schema.OneofCharacter ToSchema()
        {
            return new Schema.OneofCharacter
            {
                Keep = new Schema.Keep()
                {
                    Building = base.ToSchema(),
                }
            };
        }

        public Keep(Context context, int alliance) : base(context, alliance)
        {
        }
    }
}