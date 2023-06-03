using Core;

namespace Schema
{
    public class GuardTower : Character
    {
        public override CharacterType Type => CharacterType.GuardTower;

        protected override Core.Entity BuildCoreObject(Context context)
        {
            return new Core.GuardTower(context, this.Alliance);
        }
    }
}
