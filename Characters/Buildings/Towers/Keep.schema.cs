using Core;

namespace Schema
{
    public class Keep : Character
    {
        public override CharacterType Type => CharacterType.Keep;

        protected override Core.Entity BuildCoreObject(Context context)
        {
            return new Core.Keep(context, this.Alliance);
        }
    }
}
