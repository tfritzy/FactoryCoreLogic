using System;
using System.Collections.Generic;
using System.Net;

namespace Core
{
    public class ItemPort : Component
    {
        public override ComponentType Type => ComponentType.ItemPort;
        public const float DepositPoint = -.5f;
        public List<int> OutputSideOffsets { get; private set; }
        public List<int> InputSideOffsets { get; private set; }
        private Building BuildingOwner => (Building)Owner;

        public ItemPort(
            Entity owner,
            List<int>? outputSideOffsets = null,
            List<int>? inputSideOffsets = null) : base(owner)
        {
            OutputSideOffsets = outputSideOffsets ?? new List<int>();
            InputSideOffsets = inputSideOffsets ?? new List<int>();
        }

        public override Schema.Component ToSchema()
        {
            return new Schema.ItemOutput()
            {
                OutputSideOffsets = OutputSideOffsets
            };
        }

        public override void OnAddToGrid()
        {
            base.OnAddToGrid();

            TellConveyorsIExist();
        }

        private void TellConveyorsIExist()
        {
            for (int i = 0; i < 6; i++)
            {
                Point2Int neighbor = (Point2Int)GridHelpers.GetNeighbor(BuildingOwner.GridPosition, (HexSide)i);
                var building = Owner.Context.World.GetBuildingAt(neighbor);
                if (building?.Conveyor != null)
                {
                    building.Conveyor.FindNeighborConveyors();
                }
            }
        }

        private float checkCooldown = 0f;
        public override void Tick(float deltaTime)
        {
            base.Tick(deltaTime);

            checkCooldown -= deltaTime;
            if (checkCooldown > 0)
            {
                return;
            }
            checkCooldown = .1f;

            DepositItems();
        }

        public bool TryGiveItem(Item item)
        {
            if (Owner.Inventory == null)
            {
                var depositTarget = GetDepositTarget(item);
                if (depositTarget != null)
                {
                    depositTarget.AddItem(item);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                if (Owner.Inventory.CanAddItem(item))
                {
                    var depositTarget = GetDepositTarget(item);
                    if (depositTarget != null)
                    {
                        depositTarget.AddItem(item, DepositPoint);
                    }
                    else
                    {
                        Owner.Inventory.AddItem(item);
                    }

                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        private ConveyorComponent? GetDepositTarget(Item item)
        {
            foreach (int offset in OutputSideOffsets)
            {
                HexSide outputSide = GridHelpers.Rotate60(BuildingOwner.Rotation, offset);
                ConveyorComponent? next = Owner.Context.World.GetBuildingAt(
                    GridHelpers.GetNeighbor((Point2Int)((Building)Owner).GridPosition, outputSide)
                )?.Conveyor;

                if (next == null || next.Prev != Owner)
                {
                    continue;
                }

                if (next.Owner.IsPreview)
                {
                    continue;
                }

                if (item == null)
                {
                    return null;
                }

                if (next.CanAcceptItem(item, DepositPoint))
                {
                    return next;
                }
            }

            return null;
        }

        private void DepositItems()
        {
            if (Owner.Inventory == null)
            {
                return;
            }

            var item = Owner.Inventory.FindItem();
            if (item == null)
            {
                return;
            }

            ConveyorComponent? depositTarget = GetDepositTarget(item);
            if (depositTarget != null)
            {
                Item singleQuantity = Item.Create(item.Type);
                Owner.Inventory.RemoveCount(item.Type, 1);
                depositTarget.AddItem(singleQuantity, DepositPoint);
            }
        }
    }
}