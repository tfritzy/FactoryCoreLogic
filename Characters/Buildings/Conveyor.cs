using System.Collections.Generic;
using FactoryCore;

namespace FactoryCore
{
    public class Conveyor : Building
    {
        public Conveyor(World world) : base(world) { }
        public ConveyorCell? Cell => this.GetCell<ConveyorCell>(CellType.Conveyor);

        protected override void InitCells()
        {
            this.Cells = new Dictionary<CellType, Cell>
        {
            { CellType.Conveyor, new ConveyorCell(this) }
        };
        }
    }
}
