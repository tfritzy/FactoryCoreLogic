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

        public Conveyor(Context context) : base(context) { }

        protected override void InitCells()
        {
            this.Cells = new Dictionary<Type, Cell>
            {
                { typeof(ConveyorCell), new ConveyorCell(this) }
            };
        }
    }
}
