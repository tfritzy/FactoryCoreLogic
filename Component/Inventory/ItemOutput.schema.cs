using System;
using System.Collections.Generic;
using System.Linq;
using Core;
using Newtonsoft.Json;

namespace Schema
{
    public class TransferToConveyor : Inventory
    {
        public override ComponentType Type => ComponentType.ItemOutput;

        [JsonProperty("Sides")]
        public List<int> OutputSideOffsets = new();

        public override Core.Component FromSchema(object[] context)
        {
            Core.Entity owner = (Core.Entity)context[0];
            return new Core.ItemOutput(owner, OutputSideOffsets);
        }
    }
}
