using Core;

namespace Schema
{
    public class Conveyor : Character
    {
        public override CharacterType Type => CharacterType.Conveyor;

        public override Core.Character FromSchema(params object[] context)
        {
            return this.ToCore(context);
        }
    }
}
