using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Core
{
    public class ConveyorComponent : Component
    {
        public new Building Owner => (Building)base.Owner;
        public LinkedList<ItemOnBelt> Items;
        public override ComponentType Type => ComponentType.Conveyor;
        public HexSide? NextSide;
        public HexSide? PrevSide;
        public bool BlockPassthrough { get; private set; }

        protected Character OwnerCharacter =>
            Owner is Character ?
                (Character)Owner :
                throw new Exception("The owner of a conveyorcell must be a character");

        public ConveyorComponent? Next => NextSide.HasValue ?
            World.GetBuildingAt(
                GridHelpers.GetNeighbor(
                    (Point2Int)OwnerCharacter.GridPosition,
                    NextSide.Value)
                )?.GetComponent<ConveyorComponent>() : null;
        public ConveyorComponent? Prev => PrevSide.HasValue ?
            World.GetBuildingAt(
                GridHelpers.GetNeighbor(
                    (Point2Int)OwnerCharacter.GridPosition,
                    PrevSide.Value)
                )?.GetComponent<ConveyorComponent>() : null;
        public const float MOVEMENT_SPEED_M_S = .5f;
        public const float STRAIGHT_DISTANCE = Constants.HEX_APOTHEM * 2;
        public const float CURVE_DISTANCE = STRAIGHT_DISTANCE;

        public ConveyorComponent(Character owner, bool blockPassthrough = false) : base(owner)
        {
            Items = new LinkedList<ItemOnBelt>();
            BlockPassthrough = blockPassthrough;
        }

        public bool IsCurved()
        {
            if (Prev != null && Next != null)
            {
                int? angle = AngleBetweenThreePoints(
                    (Point2Int)Prev.OwnerCharacter.GridPosition,
                    (Point2Int)OwnerCharacter.GridPosition,
                    (Point2Int)Next.OwnerCharacter.GridPosition);

                if (angle == 2 || angle == 4)
                {
                    return true;
                }
            }

            return false;
        }

        public float GetTotalDistance()
        {
            if (IsCurved())
            {
                return CURVE_DISTANCE;
            }
            else
            {
                return STRAIGHT_DISTANCE;
            }
        }

        public override void Tick(float deltaTime)
        {
            if (Disabled)
            {
                return;
            }

            float movementAmount = MOVEMENT_SPEED_M_S * deltaTime;

            var current = Items.Last;
            while (current != null)
            {
                ItemOnBelt item = current.Value;
                float maxPosition = GetMaxPositionOfItem(current);
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
                if (BlockPassthrough && item.Value.ProgressMeters <= GetTotalDistance() / 2)
                {
                    return GetTotalDistance() / 2;
                }

                float? minBoundsOfNextItem = Next?.MinBoundsOfFirstItem();

                // If the next conveyor's first item overlaps the end of this conveyor, it is the limiter.
                if (minBoundsOfNextItem != null && minBoundsOfNextItem.Value < 0)
                {
                    return minBoundsOfNextItem.Value + GetTotalDistance() - item.Value.Item.Width / 2;
                }

                if (Next == null)
                {
                    return GetTotalDistance() - item.Value.Item.Width / 2;
                }
                else
                {
                    // Only extend past the end of the conveyor if there is a next one.
                    return GetTotalDistance();
                }
            }
            else
            {
                var nextItem = item.Next.Value;
                float nextItemProgress = item.Next.Value.ProgressMeters;
                float maxIfLimitedByNext = nextItemProgress - nextItem.Item.Width / 2 - item.Value.Item.Width / 2;

                if (BlockPassthrough && item.Value.ProgressMeters < GetTotalDistance() / 2)
                {
                    return Math.Min(GetTotalDistance() / 2, maxIfLimitedByNext);
                }
                else
                {
                    return maxIfLimitedByNext;
                }
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

            float? minBoundsOfFirstItem = MinBoundsOfFirstItem();
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
                    (Point2Int)OwnerCharacter.GridPosition,
                    (Point2Int)conveyor.OwnerCharacter.GridPosition,
                    (Point2Int)conveyor.Next.OwnerCharacter.GridPosition) < 2)
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
                    (Point2Int)OwnerCharacter.GridPosition,
                    (Point2Int)conveyor.OwnerCharacter.GridPosition,
                    (Point2Int)conveyor.Prev.OwnerCharacter.GridPosition) < 2)
                {
                    return false;
                }
            }

            return true;
        }

        public static int? AngleBetweenThreePoints(Point2Int a, Point2Int b, Point2Int c)
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
            NextSide = outputDirection;
            conveyorCell.PrevSide = GridHelpers.OppositeSide(outputDirection);
        }

        private void DisconnectNext()
        {
            if (Next != null)
            {
                Next.PrevSide = null;
            }

            NextSide = null;
        }

        private void DisconnectPrev()
        {
            if (Prev != null)
            {
                Prev.NextSide = null;
            }

            PrevSide = null;
        }

        public void FindNeighborConveyors()
        {
            for (int i = 0; i < 6; i++)
            {
                var neighborPos = GridHelpers.GetNeighbor((Point2Int)OwnerCharacter.GridPosition, (HexSide)i);
                var neighbor = World.GetBuildingAt(neighborPos);

                ConveyorComponent? neighborCell = neighbor?.GetComponent<ConveyorComponent>();

                if (neighborCell != null && CanBePrev(neighborCell))
                {
                    neighborCell.LinkTo(this, GridHelpers.OppositeSide((HexSide)i));
                }

                if (neighborCell != null && CanBeNext(neighborCell))
                {
                    LinkTo(neighborCell, (HexSide)i);
                }
            }
        }

        public override Schema.Component ToSchema()
        {
            return new Schema.ConveyorComponent()
            {
                Items = new LinkedList<Schema.ItemOnBelt>(Items.Select(x => x.ToSchema())),
                NextSide = NextSide,
                PrevSide = PrevSide,
                BlocksPassthrough = BlockPassthrough,
            };
        }

        public int GetRotation()
        {
            return 0;
        }
    }
}