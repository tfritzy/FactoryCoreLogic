using System;
using System.Linq;
using Core;
using Newtonsoft.Json;

namespace Schema
{
    public abstract class Worksite : Component
    {
        protected abstract Core.Worksite BuildWorksite(Core.Entity owner);

        public override Core.Component FromSchema(object[] context)
        {
            if (context.Length == 0 || context[0] == null || !(context[0] is Core.Entity))
                throw new ArgumentException("WorksiteComponent requires an Entity as context[0]");

            Core.Entity owner = (Core.Entity)context[0];

            return BuildWorksite(owner);
        }
    }
}
