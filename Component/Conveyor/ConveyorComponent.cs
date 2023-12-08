using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        public int Version { get; private set; }

        public ConveyorComponent? Next => NextSide.HasValue ?
            World.GetBuildingAt(
                GridHelpers.GetNeighbor(
                    (Point2Int)Owner.GridPosition,
                    NextSide.Value)
                )?.GetComponent<ConveyorComponent>() : null;
        public ConveyorComponent? Prev => PrevSide.HasValue ?
            World.GetBuildingAt(
                GridHelpers.GetNeighbor(
                    (Point2Int)Owner.GridPosition,
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
            if (Prev != null)
            {
                var nextPos = GridHelpers
                    .GetNeighbor((Point2Int)Owner.GridPosition, Owner.Rotation);
                int? angle = AngleBetweenThreePoints(
                    (Point2Int)Prev.Owner.GridPosition,
                    (Point2Int)Owner.GridPosition,
                    nextPos);

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
                float maxPosition = GetMaxPositionOfItem(current, current.Next);
                float? maxPosOnNext = Next?.GetMaxPositionOfItem(current, Next.Items.First);
                item.ProgressMeters += movementAmount;
                if (item.ProgressMeters >= GetTotalDistance() && maxPosition >= GetTotalDistance())
                {
                    if (Next != null && maxPosOnNext != null)
                    {
                        float desiredDist = item.ProgressMeters - GetTotalDistance();
                        float insertDist = MathF.Min(desiredDist, maxPosOnNext.Value);

                        if (Next.CanAcceptItem(item.Item, insertDist))
                        {
                            Next.AddItem(item.Item, insertDist);
                            Items.Remove(current);
                            Version++;
                            current = current.Previous;
                            continue;
                        }
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

        public float GetMaxPositionOfItem(LinkedListNode<ItemOnBelt> item, LinkedListNode<ItemOnBelt>? nextItem)
        {
            if (nextItem == null)
            {
                if (BlockPassthrough && item.Value.ProgressMeters <= GetTotalDistance() / 2)
                {
                    return GetTotalDistance() / 2;
                }

                float? minBoundsOfNextItem = Next?.MinBoundsOfFirstItem();

                // If the next conveyor's first item overlaps the end of this conveyor, it is the limiter.
                if (minBoundsOfNextItem != null && minBoundsOfNextItem.Value < 0)
                {
                    return minBoundsOfNextItem.Value + GetTotalDistance() - item.Value.Item.Width / 2 - .0001f;
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
                float nextItemProgress = nextItem.Value.ProgressMeters;
                float maxIfLimitedByNext =
                    nextItemProgress
                    - nextItem.Value.Item.Width / 2
                    - item.Value.Item.Width / 2
                    - .0001f;

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

        public bool CanAcceptItem(Item item, float atPoint = 0f)
        {
            return GetInsertionIndex(item, atPoint) != -1;
        }

        private int GetInsertionIndex(Item item, float atPoint)
        {
            if (atPoint < 0)
            {
                return -1;
            }

            int insertionIndex = 0;
            float itemMin = atPoint - item.Width / 2;
            float itemMax = atPoint + item.Width / 2;

            var iter = Items.First;
            while (iter != null)
            {
                if (itemMin >= iter.Value.Min && itemMin <= iter.Value.Max)
                {
                    return -1;
                }

                if (itemMax >= iter.Value.Min && itemMax <= iter.Value.Max)
                {
                    return -1;
                }

                if (iter.Value.Min > itemMax)
                {
                    break;
                }

                insertionIndex += 1;
                iter = iter.Next;
            }

            return insertionIndex;
        }

        public void AddItem(Item item, float atPoint = 0f)
        {
            int insertionIndex = GetInsertionIndex(item, atPoint);
            if (insertionIndex == -1)
            {
                throw new Exception("Cannot accept item.");
            }

            Version++;
            if (insertionIndex > 0)
            {
                var iter = Items.First;
                for (int i = 0; i < insertionIndex - 1; i++)
                {
                    iter = iter?.Next;
                }
                Items.AddAfter(iter!, new ItemOnBelt(item, atPoint));
            }
            else
            {
                Items.AddFirst(new ItemOnBelt(item, atPoint));
            }

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

            HexSide nextSide = (HexSide)conveyor.Owner.Rotation;
            var nextPos = GridHelpers
                .GetNeighbor((Point2Int)conveyor.Owner.GridPosition, nextSide);
            var angle = AngleBetweenThreePoints(
                (Point2Int)Owner.GridPosition,
                (Point2Int)conveyor.Owner.GridPosition,
                nextPos);
            if (angle < 2 || angle > 4)
            {
                return false;
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

            if (GridHelpers.GetNeighbor(
                (Point2Int)conveyor.Owner.GridPosition, conveyor.Owner.Rotation) !=
                (Point2Int)Owner.GridPosition)
            {
                return false;
            }

            HexSide prevSide = GridHelpers.OppositeSide((HexSide)Owner.Rotation);
            var prevPos = GridHelpers
                .GetNeighbor((Point2Int)conveyor.Owner.GridPosition, prevSide);
            var angle = AngleBetweenThreePoints(
                (Point2Int)Owner.GridPosition,
                (Point2Int)conveyor.Owner.GridPosition,
                prevPos);
            if (angle < 2 || angle > 4)
            {
                return false;
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

        public override void OnOwnerRotationChanged(HexSide rotation)
        {
            base.OnOwnerRotationChanged(rotation);

            FindNeighborConveyors();
        }

        public void FindNeighborConveyors()
        {
            HexSide rotation = (HexSide)Owner.Rotation;
            HexSide prevSide = GridHelpers.OppositeSide(rotation);

            for (int i = -1; i < 2; i++)
            {
                var checkPrevSide = GridHelpers.Rotate60(prevSide, i);
                var prevPos = GridHelpers.GetNeighbor((Point2Int)Owner.GridPosition, checkPrevSide);
                var checkPrev = World.GetBuildingAt(prevPos)?.GetComponent<ConveyorComponent>();
                if (checkPrev != null && CanBePrev(checkPrev))
                {
                    checkPrev.LinkTo(this, GridHelpers.OppositeSide(checkPrevSide));
                    break;
                }
            }

            var nextPos = GridHelpers
                .GetNeighbor((Point2Int)Owner.GridPosition, rotation);
            var checkNext = World.GetBuildingAt(nextPos)?.GetComponent<ConveyorComponent>();
            if (checkNext != null && CanBeNext(checkNext))
            {
                LinkTo(checkNext, rotation);
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