using Core;

namespace Schema
{
    public class Quarry : Character
    {
        public override CharacterType Type => CharacterType.Quarry;

        protected override Core.Character BuildCoreObject(Context context)
        {
            return new Core.Quarry(context, this.Alliance);
        }
    }
}
