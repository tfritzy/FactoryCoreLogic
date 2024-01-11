namespace Core
{
    public class Dummy : Unit
    {
        public override CharacterType Type => CharacterType.Dummy;
        private static readonly string name = "Dummy";
        public override string Name => name;

        protected override void InitComponents() { }

        public override Schema.OneofCharacter Serialize()
        {
            return new Schema.OneofCharacter
            {
                Dummy = new Schema.Dummy()
                {
                    Unit = base.ToSchema(),
                }
            };
        }

        public Dummy(Context context, Schema.Dummy dummy) : base(dummy.Unit, context)
        {
        }

        public Dummy(Context context, int alliance) : base(context, alliance) { }
    }
}