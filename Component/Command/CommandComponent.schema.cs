namespace Schema
{
    public class CommandComponent : Component
    {
        public override Core.ComponentType Type => Core.ComponentType.Command;

        public override Core.Component FromSchema(params object[] context)
        {
            return new Core.CommandComponent((Core.Unit)context[0]);
        }
    }
}