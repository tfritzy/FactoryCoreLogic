using System;
using System.Collections.Generic;

namespace FactoryCore
{
    public class ConveyorCell : Cell
    {
        public List<ItemOnBelt> Items { get; private set; }
        public ConveyorCell Next { get; private set; }
        public ConveyorCell Prev { get; private set; }
        public override CellType Type => CellType.Conveyor;

        public class ItemOnBelt
        {
            public Item Item;
            public int ProgressInTicks;
        }

        public ConveyorCell(Character owner) : base(owner)
        {
            Items = new List<ItemOnBelt>();
        }

        public override void Tick()
        {
            for (int i = 0; i < Items.Count; i++)
            {
                var item = Items[i];
                item.ProgressInTicks += 1;
            }
        }

        public void AddItem(Item item)
        {
            Items.Add(
                new ItemOnBelt
                {
                    Item = item,
                    ProgressInTicks = 0
                }
            );
        }

        public List<ItemOnBelt> GetItems()
        {
            return Items;
        }

        public override void OnAddToGrid()
        {
            base.OnAddToGrid();

            FindNeighborConveyors();
        }

        public bool CanBeNext(Building building)
        {
            if (building == null)
            {
                return false;
            }

            // if (this.Next != null)
            // {
            //     return false;
            // }

            if (building.GetCell<ConveyorCell>(CellType.Conveyor) == null)
            {
                return false;
            }

            if (building.GetCell<ConveyorCell>(CellType.Conveyor).Prev != null)
            {
                return false;
            }

            if (building.GetCell<ConveyorCell>(CellType.Conveyor).Next == this)
            {
                return false;
            }

            return true;
        }

        public bool CanBePrev(Building building)
        {
            if (building == null)
            {
                return false;
            }

            // if (this.Prev != null)
            // {
            //     return false;
            // }

            if (building.GetCell<ConveyorCell>(CellType.Conveyor) == null)
            {
                return false;
            }

            if (building.GetCell<ConveyorCell>(CellType.Conveyor).Next != null)
            {
                return false;
            }

            return true;
        }

        private void LinkTo(ConveyorCell conveyorCell)
        {
            this.Next = conveyorCell;
            conveyorCell.Prev = this;
        }

        public void FindNeighborConveyors()
        {
            for (int i = 0; i < 6; i++)
            {
                var neighborPos = HexGridHelpers.GetNeighbor(this.Owner.GridPosition, (HexSide)i);
                var neighbor = this.World.GetBuildingAt(neighborPos);

                if (CanBePrev(neighbor))
                {
                    neighbor.GetCell<ConveyorCell>(CellType.Conveyor).LinkTo(this);
                }

                if (CanBeNext(neighbor))
                {
                    this.LinkTo(neighbor.GetCell<ConveyorCell>(CellType.Conveyor));
                }
            }
        }
    }
}