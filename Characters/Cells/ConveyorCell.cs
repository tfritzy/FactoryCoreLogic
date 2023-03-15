using System;
using System.Collections.Generic;

namespace FactoryCore
{
    public class ConveyorCell : Cell
    {
        public List<ItemOnBelt> Items { get; private set; }
        public ConveyorCell? Next { get; private set; }
        public ConveyorCell? Prev { get; private set; }
        public override CellType Type => CellType.Conveyor;

        public class ItemOnBelt
        {
            public Item Item;
            public int ProgressInTicks;

            public ItemOnBelt(Item item)
            {
                this.Item = item;
                this.ProgressInTicks = 0;
            }
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
                new ItemOnBelt(item)
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

        public override void OnRemoveFromGrid()
        {
            base.OnRemoveFromGrid();

            DisconnectNext();
            DisconnectPrev();
        }

        public bool CanBeNext(ConveyorCell conveyor)
        {
            // if (this.Next != null)
            // {
            //     return false;
            // }

            if (conveyor == null)
            {
                return false;
            }

            if (conveyor.Prev != null)
            {
                return false;
            }

            if (conveyor.Next == this)
            {
                return false;
            }

            return true;
        }

        public bool CanBePrev(ConveyorCell conveyor)
        {
            // if (this.Prev != null)
            // {
            //     return false;
            // }

            if (conveyor == null)
            {
                return false;
            }

            if (conveyor.Next != null)
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

        private void DisconnectNext()
        {
            if (this.Next != null)
            {
                this.Next.Prev = null;
            }

            this.Next = null;
        }

        private void DisconnectPrev()
        {
            if (this.Prev != null)
            {
                this.Prev.Next = null;
            }

            this.Prev = null;
        }

        public void FindNeighborConveyors()
        {
            for (int i = 0; i < 6; i++)
            {
                var neighborPos = HexGridHelpers.GetNeighbor(this.Owner.GridPosition, (HexSide)i);
                var neighbor = this.World.GetBuildingAt(neighborPos);

                ConveyorCell? neighborCell = neighbor?.GetCell<ConveyorCell>(CellType.Conveyor);

                if (neighborCell != null && CanBePrev(neighborCell))
                {
                    neighborCell.LinkTo(this);
                }

                if (neighborCell != null && CanBeNext(neighborCell))
                {
                    this.LinkTo(neighborCell);
                }
            }
        }
    }
}