using Core;

namespace Schema
{
    public class DummyBuilding : Character
    {
        public override CharacterType Type => CharacterType.DummyBuilding;

        public override Core.Character FromSchema(object[] context)
        {
            return this.ToCore(context);
        }
    }
}
