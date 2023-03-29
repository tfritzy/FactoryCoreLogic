using System.Collections.Generic;
using FactoryCore;
using Newtonsoft.Json;
using System;

namespace FactoryCore
{
    public class Conveyor : Building
    {
        public ConveyorComponent? Component => this.GetComponent<ConveyorComponent>();
        public override CharacterType Type => CharacterType.Conveyor;

        public Conveyor(Context context) : base(context) { }

        protected override void InitComponents()
        {
            this.Components = new Dictionary<Type, Component>
            {
                { typeof(ConveyorComponent), new ConveyorComponent(this) }
            };
        }
    }
}
