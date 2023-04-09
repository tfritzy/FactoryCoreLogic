using System.Collections.Generic;
using Core;
using Newtonsoft.Json;
using System;
using System.Linq;

namespace Core
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

        public override Schema.Character ToSchema()
        {
            var conveyor = new Schema.Conveyor();
            return this.PopulateSchema(conveyor);
        }
    }
}
