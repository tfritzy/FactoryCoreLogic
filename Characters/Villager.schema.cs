using Core;

namespace Schema
{
    public class Villager : Character
    {
        public override CharacterType Type => CharacterType.Villager;

        public override Core.Character FromSchema(params object[] context)
        {
            return this.ToCore(context);
        }
    }
}