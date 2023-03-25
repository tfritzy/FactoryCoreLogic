using System.Collections.Generic;
using FactoryCore;
using Newtonsoft.Json;
using System;

namespace FactoryCore
{
    public class Conveyor : Building
    {
        public ConveyorCell? Cell => this.GetCell<ConveyorCell>();
        public override CharacterType Type => CharacterType.Conveyor;

        [JsonConstructor]
        protected Conveyor() : base(null!) { }

        public Conveyor(World world) : base(world) { }

        protected override void InitCells()
        {
            this.Cells = new Dictionary<Type, Cell>
            {
                { typeof(ConveyorCell), new ConveyorCell(this) }
            };
        }
    }
}
