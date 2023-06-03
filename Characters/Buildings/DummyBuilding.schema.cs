using Core;

namespace Schema
{
    public class DummyBuilding : Character
    {
        public override CharacterType Type => CharacterType.DummyBuilding;

        protected override Core.Entity BuildCoreObject(Context context)
        {
            return new Core.DummyBuilding(context, this.Alliance);
        }
    }
}
