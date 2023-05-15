using Core;

namespace Schema
{
    public class Quarry : Character
    {
        public override CharacterType Type => CharacterType.Quarry;

        public override Core.Character FromSchema(params object[] context)
        {
            return this.ToCore(context);
        }
    }
}
