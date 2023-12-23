using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
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
        public int Version { get; private set; }

        public Building? Next => NextSide.HasValue ?
            World.GetBuildingAt(
                GridHelpers.GetNeighbor(
                    (Point2Int)Owner.GridPosition,
                    NextSide.Value)
                ) : null;
        public Building? Prev => PrevSide.HasValue ?
            World.GetBuildingAt(
                GridHelpers.GetNeighbor(
                    (Point2Int)Owner.GridPosition,
                    PrevSide.Value)
                ) : null;
        public const float MOVEMENT_SPEED_M_S = .5f;
        public const float STRAIGHT_DISTANCE = Constants.HEX_APOTHEM * 2;
        public const float CURVE_DISTANCE = STRAIGHT_DISTANCE;

        public ConveyorComponent(Character owner) : base(owner)
        {
            Items = new LinkedList<ItemOnBelt>();
        }

        public bool IsCurved()
        {
            if (Prev != null)
            {
                var nextPos = GridHelpers
                    .GetNeighbor((Point2Int)Owner.GridPosition, Owner.Rotation);
                int? angle = AngleBetweenThreePoints(
                    (Point2Int)Prev.GridPosition,
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
                float? maxPosOnNext = Next?.Conveyor?.GetMaxPositionOfItem(current, Next.Conveyor.Items.First);
                item.ProgressMeters += movementAmount;

                if (Next?.Conveyor != null)
                {
                    if (item.ProgressMeters >= GetTotalDistance() /* remove -> */ && maxPosition >= GetTotalDistance())
                    {
                        if (Next != null && maxPosOnNext != null)
                        {
                            float desiredDist = item.ProgressMeters - GetTotalDistance();
                            float insertDist = MathF.Min(desiredDist, maxPosOnNext.Value);

                            if (Next.Conveyor.CanAcceptItem(item.Item, insertDist))
                            {
                                Next.Conveyor.AddItem(item.Item, insertDist);
                                Items.Remove(current);
                                Version++;
                                current = current.Previous;
                                continue;
                            }
                        }
                    }
                }
                else if (Next?.ItemPort != null)
                {
                    if (item.ProgressMeters >= maxPosition)
                    {
                        bool wasAdded = Next.ItemPort.TryGiveItem(
                            item.Item,
                            GridHelpers.OppositeSide(NextSide!.Value));
                        if (wasAdded)
                        {
                            Items.Remove(item);
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

                if (Next == null || Next.IsPreview)
                {
                    return GetTotalDistance() - item.Value.Item.Width / 2;
                }

                float? minBoundsOfNextItem = Next.Conveyor?.MinBoundsOfFirstItem();
                // If the next conveyor's first item overlaps the end of this conveyor, it is the limiter.
                if (minBoundsOfNextItem != null && minBoundsOfNextItem.Value < 0)
                {
                    return minBoundsOfNextItem.Value + GetTotalDistance() - item.Value.Item.Width / 2 - .0001f;
                }

                // Only extend past the end of the conveyor if there is a next one.
                return GetTotalDistance();
            }
            else
            {
                float nextItemProgress = nextItem.Value.ProgressMeters;
                float maxIfLimitedByNext =
                    nextItemProgress
                    - nextItem.Value.Item.Width / 2
                    - item.Value.Item.Width / 2
                    - .0001f;
                return maxIfLimitedByNext;
            }
        }

        public bool CanAcceptItem(Item item, float atPoint = 0f)
        {
            if (Owner.IsPreview)
            {
                return false;
            }

            return GetInsertionIndex(item, atPoint) != -1;
        }

        private int GetInsertionIndex(Item item, float atPoint)
        {
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

        public bool CanBeNext(Building building)
        {
            if (building.Conveyor != null)
            {
                var conveyor = building.Conveyor;
                if (conveyor == null)
                {
                    return false;
                }

                if (conveyor.Prev != null)
                {
                    return false;
                }

                if (conveyor.Next == this.Owner)
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
            else if (building.ItemPort != null)
            {
                HexSide? neighborSide = GridHelpers.GetNeighborSide(
                    (Point2Int)Owner.GridPosition,
                    (Point2Int)building.GridPosition);

                if (neighborSide == null)
                {
                    return false;
                }

                foreach (int offset in building.ItemPort.InputSideOffsets)
                {
                    HexSide side = GridHelpers.Rotate60(building.Rotation, offset);
                    if (GridHelpers.OppositeSide(side) == neighborSide)
                    {
                        return true;
                    }
                }
            }

            return false;

        }

        public bool CanBePrev(Building building)
        {
            if (building == null)
            {
                return false;
            }

            if (building.Conveyor != null)
            {
                if (building.Conveyor.Next != null)
                {
                    return false;
                }

                if (GridHelpers.GetNeighbor(
                    (Point2Int)building.Conveyor.Owner.GridPosition, building.Conveyor.Owner.Rotation) !=
                    (Point2Int)Owner.GridPosition)
                {
                    return false;
                }

                HexSide prevSide = GridHelpers.OppositeSide(Owner.Rotation);
                var prevPos = GridHelpers
                    .GetNeighbor((Point2Int)building.Conveyor.Owner.GridPosition, prevSide);
                var angle = AngleBetweenThreePoints(
                    (Point2Int)Owner.GridPosition,
                    (Point2Int)building.Conveyor.Owner.GridPosition,
                    prevPos);
                if (angle < 2 || angle > 4)
                {
                    return false;
                }

                return true;
            }

            if (building.HasComponent<ItemPort>())
            {
                var itemOutput = building.GetComponent<ItemPort>();
                HexSide? neighborSide = GridHelpers.GetNeighborSide(
                    (Point2Int)building.GridPosition,
                    (Point2Int)Owner.GridPosition);

                if (neighborSide == null)
                {
                    return false;
                }

                foreach (int offset in itemOutput.OutputSideOffsets)
                {
                    HexSide side = GridHelpers.Rotate60(building.Rotation, offset);
                    if (side == neighborSide)
                    {
                        return true;
                    }
                }
            }

            return false;
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
            if (Next?.Conveyor != null)
            {
                Next.Conveyor.PrevSide = null;
            }

            NextSide = null;
        }

        private void DisconnectPrev()
        {
            if (Prev?.Conveyor != null)
            {
                Prev.Conveyor.NextSide = null;
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
                var checkPrev = World.GetBuildingAt(prevPos);
                if (checkPrev != null && CanBePrev(checkPrev))
                {
                    if (checkPrev.Conveyor != null)
                    {
                        checkPrev.Conveyor?.LinkTo(this, GridHelpers.OppositeSide(checkPrevSide));
                    }
                    else
                    {
                        PrevSide = checkPrevSide;
                    }

                    break;
                }
            }

            var nextPos = GridHelpers
                .GetNeighbor((Point2Int)Owner.GridPosition, rotation);
            var checkNext = World.GetBuildingAt(nextPos);
            if (checkNext != null && CanBeNext(checkNext))
            {
                if (checkNext.Conveyor != null)
                {
                    LinkTo(checkNext.Conveyor, rotation);
                }
                else
                {
                    NextSide = rotation;
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
            };
        }

        public void RemoveItem(LinkedListNode<ItemOnBelt> item)
        {
            Items.Remove(item);
            Version++;
        }

        public int GetRotation()
        {
            return 0;
        }
    }
}