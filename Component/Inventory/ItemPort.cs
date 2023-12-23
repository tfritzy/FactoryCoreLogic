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
        public Dictionary<int, ItemType> SideOffsetToFilter { get; set; }
        private Building BuildingOwner => (Building)Owner;

        public ItemPort(
            Entity owner,
            List<int>? outputSideOffsets = null,
            List<int>? inputSideOffsets = null,
            Dictionary<int, ItemType>? sideOffsetToFilter = null) : base(owner)
        {
            OutputSideOffsets = outputSideOffsets ?? new List<int>();
            InputSideOffsets = inputSideOffsets ?? new List<int>();
            SideOffsetToFilter = sideOffsetToFilter ?? new Dictionary<int, ItemType>();
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

        public bool TryGiveItem(Item item, HexSide fromSide)
        {
            int sideOffset = (int)GridHelpers.Rotate60(BuildingOwner.Rotation, (int)fromSide);

            if (!InputSideOffsets.Contains(sideOffset))
            {
                return false;
            }

            if (SideOffsetToFilter.ContainsKey(sideOffset))
            {
                if (SideOffsetToFilter[sideOffset] == item.Type)
                {
                    return false;
                }
            }

            if (Owner.Inventory == null)
            {
                var depositInfo = GetDepositSide(item);
                if (depositInfo != null)
                {
                    depositInfo.Value.Conveyor.AddItem(item);
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
                    var depositInfo = GetDepositSide(item);
                    if (depositInfo != null)
                    {
                        depositInfo.Value.Conveyor.AddItem(item, DepositPoint);
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

        struct DepositInfo
        {
            public ConveyorComponent Conveyor;
            public ItemType itemType;
        }
        private DepositInfo? GetDepositSide(Item? item)
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

                // If caller didn't specify an item, find the first item that works for this side
                Item? checkDepositItem =
                    item ??
                    Owner.Inventory?.FindWhere(
                        i => i != null &&
                            (!SideOffsetToFilter.ContainsKey(offset) ||
                             SideOffsetToFilter[offset] != i?.Type));
                if (checkDepositItem == null)
                {
                    continue;
                }

                if (next.CanAcceptItem(checkDepositItem, DepositPoint))
                {
                    return new DepositInfo
                    {
                        Conveyor = next,
                        itemType = checkDepositItem.Type
                    };
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

            DepositInfo? depositInfo = GetDepositSide(null);
            if (depositInfo != null)
            {
                Item singleQuantity = Item.Create(depositInfo.Value.itemType);
                Owner.Inventory.RemoveCount(depositInfo.Value.itemType, 1);
                depositInfo.Value.Conveyor.AddItem(singleQuantity, DepositPoint);
            }
        }
    }
}