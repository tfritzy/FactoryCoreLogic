using Newtonsoft.Json;

namespace Schema
{
    public class Smelt : Component
    {
        public override Core.ComponentType Type => Core.ComponentType.Smelt;

        public override Core.Component FromSchema(params object[] context)
        {
            var smelt = new Core.Smelt((Core.Building)context[0])
            {
            };
            return smelt;
        }
    }
}