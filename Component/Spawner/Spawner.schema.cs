namespace Schema
{
    public class Spawner : Component
    {
        public override Core.ComponentType Type => Core.ComponentType.Spawner;

        public override Core.Component FromSchema(params object[] context)
        {
            return new Core.Spawner((Core.Entity)context[0]);
        }
    }
}