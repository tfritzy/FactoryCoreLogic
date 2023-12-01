using System;
using System.Collections.Generic;
using System.Linq;
using Core;
using Newtonsoft.Json;
using static Core.Component;

namespace Schema
{
    public class ConveyorComponent : Component
    {
        public override ComponentType Type => ComponentType.Conveyor;

        [JsonProperty("items")]
        public LinkedList<ItemOnBelt>? Items { get; set; }

        [JsonProperty("nextSide")]
        public HexSide? NextSide;

        [JsonProperty("prevSide")]
        public HexSide? PrevSide;

        [JsonProperty("blocksPassthrough")]
        public bool? BlocksPassthrough;

        public override Core.Component FromSchema(object[] context)
        {
            if (context.Length == 0 || context[0] == null || !(context[0] is Core.Character))
                throw new ArgumentException("ConveyorComponent requires an FactoryCore.Character as context[0]");

            Core.Character owner = (Core.Character)context[0];

            var component = new Core.ConveyorComponent(owner, BlocksPassthrough ?? false);
            component.NextSide = NextSide;
            component.PrevSide = PrevSide;

            if (Items == null)
                throw new ArgumentException("To build a ConveyorComponent, Items must not be null.");

            component.Items = new LinkedList<Core.ItemOnBelt>(Items.Select(item => item.FromSchema()));

            return component;
        }
    }
}
