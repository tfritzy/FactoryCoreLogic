using Core;

namespace Schema
{
    public class Player : Character
    {
        public override CharacterType Type => CharacterType.Player;

        protected override Core.Entity BuildCoreObject(Context context)
        {
            return new Core.Player(context, this.Alliance);
        }
    }
}