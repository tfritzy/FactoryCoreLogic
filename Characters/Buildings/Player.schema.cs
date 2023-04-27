using Core;

namespace Schema
{
    public class Player : Character
    {
        public override CharacterType Type => CharacterType.Player;

        public override Core.Character FromSchema(params object[] context)
        {
            return this.ToCore(context);
        }
    }
}