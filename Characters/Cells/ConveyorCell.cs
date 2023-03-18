using System;
using System.Collections.Generic;

namespace FactoryCore
{
    public class ConveyorCell : Cell
    {
        public LinkedList<ItemOnBelt> Items { get; private set; }
        public ConveyorCell? Next { get; private set; }
        public ConveyorCell? Prev { get; private set; }
        public override CellType Type => CellType.Conveyor;
        public const float MOVEMENT_SPEED_M_S = .5f;
        public const float CURVE_DISTANCE = Constants.HEX_APOTHEM * 2 * .85f;

        public class ItemOnBelt
        {
            public Item Item;
            public float ProgressMeters;

            public ItemOnBelt(Item item, float progressMeters)
            {
                this.Item = item;
                this.ProgressMeters = progressMeters;
            }
        }

        public ConveyorCell(Character owner) : base(owner)
        {
            Items = new LinkedList<ItemOnBelt>();
        }

        public float GetTotalDistance()
        {
            if (Prev != null && Next != null)
            {
                int? angle = AngleBetweenThreePoints(
                    Prev.Owner.GridPosition,
                    Owner.GridPosition,
                    Next.Owner.GridPosition);

                if (angle == 2 || angle == 4)
                {
                    return CURVE_DISTANCE;
                }
                else
                {
                    return Constants.HEX_APOTHEM * 2;
                }
            }
            else
            {
                return Constants.HEX_APOTHEM * 2;
            }
        }

        public override void Tick(float deltaTime)
        {
            float movementAmount = MOVEMENT_SPEED_M_S * deltaTime;

            var current = Items.Last;
            while (current != null)
            {
                ItemOnBelt item = current.Value;

                item.ProgressMeters += movementAmount;

                if (item.ProgressMeters >= GetTotalDistance())
                {
                    if (Next != null && Next.CanAcceptItem(item.Item))
                    {
                        // TODO insert at point.
                        Next.AddItem(item.Item);
                        Items.Remove(current);
                        current = current.Previous;
                        continue;
                    }
                }

                float maxPosition = GetMaxPositionOfItem(current);
                if (item.ProgressMeters > maxPosition)
                {
                    item.ProgressMeters = maxPosition;
                }

                current = current.Previous;
            }
        }

        public float? MinBoundsOfFirstItem()
        {
            var firstItem = Items.First?.Value;
            if (firstItem == null)
            {
                return null;
            }

            return firstItem.ProgressMeters - firstItem.Item.Width / 2;
        }

        public float GetMaxPositionOfItem(LinkedListNode<ItemOnBelt> item)
        {
            if (item.Next == null)
            {
                float? minBoundsOfNextItem = this.Next?.MinBoundsOfFirstItem();

                // If the next conveyor's first item overlaps the end of this conveyor, it is the limiter.
                if (minBoundsOfNextItem != null && minBoundsOfNextItem.Value < 0)
                {
                    return minBoundsOfNextItem.Value + GetTotalDistance();
                }

                return GetTotalDistance();
            }
            else
            {
                var nextItem = item.Next.Value;
                float nextItemProgress = item.Next.Value.ProgressMeters;
                return nextItemProgress - nextItem.Item.Width / 2 - item.Value.Item.Width / 2;
            }
        }

        public bool CanAcceptItem(Item item)
        {
            var firstItem = Items.First?.Value;
            if (firstItem == null)
            {
                return true;
            }

            float minBoundsOfFirstItem = firstItem.ProgressMeters - firstItem.Item.Width / 2;
            return minBoundsOfFirstItem > item.Width / 2;
        }

        public void AddItem(Item item, float atPoint = 0f)
        {
            if (!CanAcceptItem(item))
            {
                throw new Exception("Cannot accept item.");
            }

            float? minBoundsOfFirstItem = this.MinBoundsOfFirstItem();
            if (minBoundsOfFirstItem != null)
            {
                atPoint = Math.Max(atPoint, minBoundsOfFirstItem.Value - item.Width / 2);
            }

            Items.AddFirst(new ItemOnBelt(item, atPoint));
        }

        public LinkedList<ItemOnBelt> GetItems()
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

            if (conveyor.Next != null)
            {
                if (AngleBetweenThreePoints(
                    this.Owner.GridPosition,
                    conveyor.Owner.GridPosition,
                    conveyor.Next.Owner.GridPosition) < 2)
                {
                    return false;
                }
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

            if (conveyor.Prev != null)
            {
                if (AngleBetweenThreePoints(
                    this.Owner.GridPosition,
                    conveyor.Owner.GridPosition,
                    conveyor.Prev.Owner.GridPosition) < 2)
                {
                    return false;
                }
            }

            return true;
        }

        private int? AngleBetweenThreePoints(Point2Int a, Point2Int b, Point2Int c)
        {
            HexSide? ba = HexGridHelpers.GetNeighborSide(b, a);
            HexSide? bc = HexGridHelpers.GetNeighborSide(b, c);

            if (ba == null || bc == null)
            {
                return null;
            }

            return Math.Abs((int)bc.Value - (int)ba.Value);
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