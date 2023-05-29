namespace Core
{
    public class GuardTower : Building
    {
        public override CharacterType Type => CharacterType.GuardTower;
        public static Point3Float ProjectileOffset = new Point3Float(0, 0, 3f);
        public override Point3Float ProjectileSpawnOffset => ProjectileOffset;

        public override Schema.Character ToSchema()
        {
            var guardTower = new Schema.GuardTower();
            return this.PopulateSchema(guardTower);
        }

        public GuardTower(Context context, int alliance) : base(context, alliance)
        {
        }
    }
}