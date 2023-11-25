using System.Collections.Generic;
using Core;
using Newtonsoft.Json;
using System;
using System.Linq;

namespace Core
{
    public class Conveyor : Building
    {
        public ConveyorComponent? Component => GetComponent<ConveyorComponent>();
        public override CharacterType Type => CharacterType.Conveyor;
        public override int Height => 1;
        private static readonly string name = "Conveyor";
        public override string Name => name;

        public Conveyor(Context context, int alliance) : base(context, alliance)
        {
        }

        protected override void InitComponents()
        {
            SetComponent(new ConveyorComponent(this));
        }

        public override Schema.Entity BuildSchemaObject()
        {
            return new Schema.Conveyor();
        }
    }
}
