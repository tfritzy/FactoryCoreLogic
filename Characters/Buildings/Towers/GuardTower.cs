namespace Core
{
    public class GuardTower : Building
    {
        public override CharacterType Type => CharacterType.GuardTower;
        public static Point3Float ProjectileOffset = new Point3Float(0, 0, 3f);
        public override Point3Float ProjectileSpawnOffset => ProjectileOffset;
        public override int Height => 5;
        private static readonly string name = "Guard tower";
        public override string Name => name;

        public override Schema.OneofCharacter Serialize()
        {
            return new Schema.OneofCharacter
            {
                GuardTower = new Schema.GuardTower()
                {
                    Building = base.ToSchema(),
                }
            };
        }

        public GuardTower(Context context, Schema.GuardTower guardTower) : base(guardTower.Building, context)
        {
        }

        public GuardTower(Context context, int alliance) : base(context, alliance)
        {
        }
    }
}