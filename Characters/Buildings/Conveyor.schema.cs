using Core;

namespace Schema
{
    public class Conveyor : Character
    {
        public override CharacterType Type => CharacterType.Conveyor;

        protected override Core.Entity BuildCoreObject(Context context)
        {
            return new Core.Conveyor(context, this.Alliance);
        }
    }
}
