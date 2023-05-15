using System;
using System.Linq;
using Core;
using Newtonsoft.Json;

namespace Schema
{
    public class QuarryWorksite : Worksite
    {
        public override ComponentType Type => ComponentType.QuarryWorksite;

        protected override Core.Worksite BuildWorksite(Core.Entity owner)
        {
            return new Core.QuarryWorksite(owner);
        }
    }
}
