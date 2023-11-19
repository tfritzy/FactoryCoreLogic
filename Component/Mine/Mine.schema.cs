namespace Schema
{
    public class Mine : Component
    {
        public override Core.ComponentType Type => Core.ComponentType.Mine;

        public override Core.Component FromSchema(params object[] context)
        {
            return new Core.Mine((Core.Building)context[0]);
        }
    }
}