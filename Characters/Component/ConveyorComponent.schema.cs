using System;
using System.Collections.Generic;
using Core;
using Newtonsoft.Json;
using static Core.ConveyorComponent;

namespace Schema
{
    public class ConveyorComponent : Component, Schema<Core.ConveyorComponent>
    {
        public override ComponentType Type => ComponentType.Conveyor;

        [JsonProperty("items")]
        public LinkedList<ItemOnBelt>? Items { get; private set; }

        [JsonProperty("nextSide")]
        private HexSide? NextSide;

        [JsonProperty("prevSide")]
        private HexSide? PrevSide;

        public Core.ConveyorComponent FromSchema(object[] context)
        {
            if (context.Length == 0 || context[0] == null || !(context[0] is Core.Character))
                throw new ArgumentException("ConveyorComponent requires an FactoryCore.Character as context[0]");

            Core.Character owner = (Core.Character)context[0];

            var component = new Core.ConveyorComponent(owner);
            component.NextSide = NextSide;
            component.PrevSide = PrevSide;

            if (Items == null)
                throw new ArgumentException("To build a ConveyorComponent, Items must not be null.");

            component.Items = Items;

            return component;
        }
    }
}
