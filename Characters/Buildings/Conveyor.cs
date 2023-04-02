using System.Collections.Generic;
using Core;
using Newtonsoft.Json;
using System;
using System.Linq;

namespace Core
{
    public class Conveyor : Building
    {
        public ConveyorComponent? ConveyorComponent => this.GetComponent<ConveyorComponent>();
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
            return new Schema.Conveyor()
            {
                Id = this.Id,
                GridPosition = this.GridPosition,
                Components = this.Components.ToDictionary(
                    x => Component.ComponentTypeMap[x.Key], x => x.Value.ToSchema())
            };
        }
    }
}
