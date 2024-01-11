using Newtonsoft.Json;

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
                    Character = base.ToSchema(),
                }
            };
        }

        public Dummy(Context context, Schema.Dummy dummy) : base(dummy.Character, context)
        {
        }

        public Dummy(Context context, int alliance) : base(context, alliance) { }
    }
}