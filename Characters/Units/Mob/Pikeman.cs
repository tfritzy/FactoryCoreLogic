namespace Core
{
    public class Pikeman : Mob
    {
        public override CharacterType Type => CharacterType.Pikeman;
        private static readonly string name = "Pikeman";
        public override string Name => name;

        public new Schema.OneofCharacter ToSchema()
        {
            return new Schema.OneofCharacter
            {
                Pikeman = new Schema.Pikeman()
                {
                    Character = base.ToSchema(),
                }
            };
        }

        public Pikeman(Context context, int alliance) : base(context, alliance)
        {
        }

        protected override void InitComponents()
        {
            base.InitComponents();

            SetComponent(new Life(this, 100));
            SetComponent(new Attack(this, cooldown: 1.5f, damage: 5, range: Attack.MeleeRange));
        }
    }
}