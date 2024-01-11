using System.Collections.Generic;
using Core;
using System;
using System.Linq;

namespace Core
{
    public class Conveyor : Building
    {
        public override CharacterType Type => CharacterType.Conveyor;
        public override int Height => 1;
        private static readonly string name = "Conveyor";
        public override string Name => name;

        public Conveyor(Context context, Schema.Conveyor conveyor) : base(conveyor.Building, context)
        {
        }

        public Conveyor(Context context, int alliance) : base(context, alliance)
        {
        }

        protected override void InitComponents()
        {
            SetComponent(new ConveyorComponent(this));
        }

        public override Schema.OneofCharacter Serialize()
        {
            return new Schema.OneofCharacter
            {
                Conveyor = new Schema.Conveyor()
                {
                    Building = base.ToSchema(),
                }
            };
        }
    }
}
