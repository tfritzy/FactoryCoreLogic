using Core;

namespace Schema
{
    public class Villager : Character
    {
        public override CharacterType Type => CharacterType.Villager;

        protected override Core.Entity BuildCoreObject(Context context)
        {
            return new Core.Villager(context, this.Alliance);
        }
    }
}