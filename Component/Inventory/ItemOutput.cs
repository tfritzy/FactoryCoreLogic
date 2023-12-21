using System;
using System.Collections.Generic;

namespace Core
{
    public class ItemOutput : Component
    {
        public override ComponentType Type => ComponentType.ItemOutput;
        public const float DepositPoint = -.5f;
        public List<int> OutputSideOffsets { get; private set; }
        private Building BuildingOwner => (Building)Owner;

        public ItemOutput(Entity owner, List<int> outputSideOffsets) : base(owner)
        {
            OutputSideOffsets = outputSideOffsets;
        }

        public override Schema.Component ToSchema()
        {
            return new Schema.TransferToConveyor()
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

            if (Owner.Inventory == null)
            {
                return;
            }

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

                var item = Owner.Inventory.FindItem();
                if (item == null)
                {
                    return;
                }

                float dropPoint = DepositPoint;
                if (next.CanAcceptItem(item, dropPoint))
                {
                    Owner.Inventory.RemoveCount(item.Type, 1);
                    next.AddItem(item, dropPoint);
                }
                else
                {
                    Console.WriteLine("Next can't accept item");
                }
            }
        }
    }
}