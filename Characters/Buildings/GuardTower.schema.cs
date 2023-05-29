using Core;

namespace Schema
{
    public class GuardTower : Character
    {
        public override CharacterType Type => CharacterType.GuardTower;

        public override Core.Character FromSchema(params object[] context)
        {
            return this.ToCore(context);
        }
    }
}
