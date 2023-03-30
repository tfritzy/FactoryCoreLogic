using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;

namespace FactoryCore
{
    public class ConveyorComponent : Component
    {
        public LinkedList<ItemOnBelt> Items;
        public override ComponentType Type => ComponentType.Conveyor;
        public HexSide? NextSide;
        public HexSide? PrevSide;

        protected Character OwnerCharacter =>
            this.Owner is Character ?
                (Character)this.Owner :
                throw new Exception("The owner of a conveyorcell must be a character");

        public ConveyorComponent? Next => NextSide.HasValue ?
            World.GetBuildingAt(
                GridHelpers.GetNeighbor(
                    OwnerCharacter.GridPosition,
                    NextSide.Value)
                )?.GetComponent<ConveyorComponent>() : null;
        public ConveyorComponent? Prev => PrevSide.HasValue ?
            World.GetBuildingAt(
                GridHelpers.GetNeighbor(
                    OwnerCharacter.GridPosition,
                    PrevSide.Value)
                )?.GetComponent<ConveyorComponent>() : null;
        public const float MOVEMENT_SPEED_M_S = .5f;
        public const float STRAIGHT_DISTANCE = Constants.HEX_APOTHEM * 2;
        public const float CURVE_DISTANCE = Constants.HEX_APOTHEM * 2 * .85f;

        [JsonObject(MemberSerialization.OptIn)]
        public class ItemOnBelt
        {
            [JsonProperty("item")]
            public Item Item;

            [JsonProperty("progressMeters")]
            public float ProgressMeters;

            public ItemOnBelt(Item item, float progressMeters)
            {
                this.Item = item;
                this.ProgressMeters = progressMeters;
            }
        }

        public ConveyorComponent(Character owner) : base(owner)
        {
            Items = new LinkedList<ItemOnBelt>();
        }

        public float GetTotalDistance()
        {
            if (Prev != null && Next != null)
            {
                int? angle = AngleBetweenThreePoints(
                    Prev.OwnerCharacter.GridPosition,
                    OwnerCharacter.GridPosition,
                    Next.OwnerCharacter.GridPosition);

                if (angle == 2 || angle == 4)
                {
                    return CURVE_DISTANCE;
                }
            }

            return STRAIGHT_DISTANCE;
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
                        Next.AddItem(item.Item, item.ProgressMeters - GetTotalDistance());
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
                atPoint = Math.Min(atPoint, minBoundsOfFirstItem.Value - item.Width / 2);
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

        public bool CanBeNext(ConveyorComponent conveyor)
        {
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
                    this.OwnerCharacter.GridPosition,
                    conveyor.OwnerCharacter.GridPosition,
                    conveyor.Next.OwnerCharacter.GridPosition) < 2)
                {
                    return false;
                }
            }

            return true;
        }

        public bool CanBePrev(ConveyorComponent conveyor)
        {
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
                    this.OwnerCharacter.GridPosition,
                    conveyor.OwnerCharacter.GridPosition,
                    conveyor.Prev.OwnerCharacter.GridPosition) < 2)
                {
                    return false;
                }
            }

            return true;
        }

        private int? AngleBetweenThreePoints(Point2Int a, Point2Int b, Point2Int c)
        {
            HexSide? ba = GridHelpers.GetNeighborSide(b, a);
            HexSide? bc = GridHelpers.GetNeighborSide(b, c);

            if (ba == null || bc == null)
            {
                return null;
            }

            return Math.Abs((int)bc.Value - (int)ba.Value);
        }

        private void LinkTo(ConveyorComponent conveyorCell, HexSide outputDirection)
        {
            this.NextSide = outputDirection;
            conveyorCell.PrevSide = GridHelpers.OppositeSide(outputDirection);
        }

        private void DisconnectNext()
        {
            if (this.Next != null)
            {
                this.Next.PrevSide = null;
            }

            this.NextSide = null;
        }

        private void DisconnectPrev()
        {
            if (this.Prev != null)
            {
                this.Prev.NextSide = null;
            }

            this.PrevSide = null;
        }

        public void FindNeighborConveyors()
        {
            for (int i = 0; i < 6; i++)
            {
                var neighborPos = GridHelpers.GetNeighbor(this.OwnerCharacter.GridPosition, (HexSide)i);
                var neighbor = this.World.GetBuildingAt(neighborPos);

                ConveyorComponent? neighborCell = neighbor?.GetComponent<ConveyorComponent>();

                if (neighborCell != null && CanBePrev(neighborCell))
                {
                    neighborCell.LinkTo(this, GridHelpers.OppositeSide((HexSide)i));
                }

                if (neighborCell != null && CanBeNext(neighborCell))
                {
                    this.LinkTo(neighborCell, (HexSide)i);
                }
            }
        }
    }
}