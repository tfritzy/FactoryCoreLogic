using System;
using System.Collections.Generic;
using FactoryCore;
using Newtonsoft.Json;
using static FactoryCore.ConveyorComponent;

namespace Schema
{
    public class ConveyorComponent : Component, ISchema<FactoryCore.ConveyorComponent>
    {
        [JsonProperty("items")]
        public LinkedList<ItemOnBelt>? Items { get; private set; }

        [JsonProperty("nextSide")]
        private HexSide? NextSide;

        [JsonProperty("prevSide")]
        private HexSide? PrevSide;

        public FactoryCore.ConveyorComponent FromSchema(object[] context)
        {
            if (context.Length == 0 || context[0] == null || !(context[0] is Character))
                throw new ArgumentException("ConveyorComponent requires an Character as context[0]");

            Character owner = (Character)context[0];

            var component = new FactoryCore.ConveyorComponent(owner);
            component.NextSide = NextSide;
            component.PrevSide = PrevSide;

            if (Items == null)
                throw new ArgumentException("To build a ConveyorComponent, Items must not be null.");

            component.Items = Items;

            return component;
        }
    }
}
