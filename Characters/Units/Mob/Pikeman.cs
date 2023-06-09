namespace Core
{
    public class Pikeman : Mob
    {
        public override CharacterType Type => CharacterType.Pikeman;

        public override Schema.Entity BuildSchemaObject()
        {
            return new Schema.Pikeman();
        }

        public Pikeman(Context context, int alliance) : base(context, alliance)
        {
        }

        protected override void InitComponents()
        {
            base.InitComponents();

            SetComponent(new Life(this, 100));
            SetComponent(new Attack(this, cooldown: 5f, damage: 50, range: 1f));
        }
    }
}