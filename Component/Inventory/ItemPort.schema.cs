using System;
using System.Collections.Generic;
using System.Linq;
using Core;
using Newtonsoft.Json;

namespace Schema
{
    public class ItemOutput : Component
    {
        public override ComponentType Type => ComponentType.ItemPort;

        public override Core.Component FromSchema(object[] context)
        {
            Core.Entity owner = (Core.Entity)context[0];
            return new Core.ItemPort(owner);
        }
    }
}
