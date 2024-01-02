using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace Core
{
    public class ItemPort : Component
    {
        public override ComponentType Type => ComponentType.ItemPort;
        public const float DepositPoint = 0f;
        public List<int> OutputSideOffsets;
        public List<int> InputSideOffsets;
        private Dictionary<int, ItemType> SideToOnlyAllowedItem;
        private Dictionary<ItemType, int> ItemToOnlyAllowedSide;
        public Func<Item, Inventory?> GetDestinationForItem;
        private Building BuildingOwner => (Building)Owner;

        public ItemPort(Entity owner) : base(owner)
        {
            OutputSideOffsets = new List<int>();
            InputSideOffsets = new List<int>();
            SideToOnlyAllowedItem = new Dictionary<int, ItemType>();
            ItemToOnlyAllowedSide = new Dictionary<ItemType, int>();
            GetDestinationForItem = (item) => Owner.Inventory;
        }

        public override Schema.Component ToSchema()
        {
            return new Schema.ItemOutput() { };
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
            int sideOffset = GridHelpers.HexDelta(BuildingOwner.Rotation, fromSide);
            if (!InputSideOffsets.Contains(sideOffset))
            {
                return false;
            }

            Inventory? targetInventory = GetDestinationForItem(item);

            // Deposit isn't attempted from other inventories.
            if (targetInventory == Owner.Inventory)
            {
                bool deposited = TryDeposit(item);
                if (deposited)
                {
                    return true;
                }
            }

            if (targetInventory != null)
            {
                if (targetInventory.CanAddItem(item))
                {
                    targetInventory.AddItem(item);
                    return true;
                }
            }

            return false;
        }

        private bool TryDeposit(Item? item)
        {
            foreach (int offset in OutputSideOffsets)
            {
                if (item != null)
                {
                    if (SideToOnlyAllowedItem.ContainsKey(offset) && SideToOnlyAllowedItem[offset] != item.Type)
                    {
                        continue;
                    }

                    if (ItemToOnlyAllowedSide.ContainsKey(item.Type) && ItemToOnlyAllowedSide[item.Type] != offset)
                    {
                        continue;
                    }
                }

                // If caller didn't specify an item, find the first item that works for this side
                Item? checkDepositItem = item;
                bool itemFromInventory = false;
                if (checkDepositItem == null)
                {
                    itemFromInventory = true;
                    checkDepositItem = Owner.Inventory?.FindWhere(
                        i => i != null &&
                        (!SideToOnlyAllowedItem.ContainsKey(offset) ||
                        SideToOnlyAllowedItem[offset] != i?.Type));
                }

                if (checkDepositItem == null)
                {
                    continue;
                }

                HexSide outputSide = GridHelpers.Rotate60(BuildingOwner.Rotation, offset);
                Building? nextBuilding = Owner.Context.World.GetBuildingAt(
                    GridHelpers.GetNeighbor((Point2Int)((Building)Owner).GridPosition, outputSide)
                );

                if (nextBuilding == null || nextBuilding.IsPreview)
                {
                    continue;
                }

                if (nextBuilding?.Conveyor != null)
                {
                    ConveyorComponent next = nextBuilding.Conveyor;
                    if (next == null || next.Prev != Owner)
                    {
                        continue;
                    }

                    if (next.CanAcceptItem(checkDepositItem, DepositPoint))
                    {
                        Item singleQuantity = Item.Create(checkDepositItem.Type);
                        ulong quantity = checkDepositItem.Units == Item.UnitType.Milligram ? 10_000_000u : 1u;
                        quantity = Math.Min(checkDepositItem.Quantity, quantity);
                        singleQuantity.SetQuantity(quantity);
                        next.AddItem(singleQuantity, DepositPoint);
                        if (itemFromInventory)
                            Owner.Inventory?.RemoveCount(checkDepositItem.Type, quantity);
                        return true;
                    }
                }
                else if (nextBuilding?.ItemPort != null)
                {
                    ItemPort next = nextBuilding.ItemPort;
                    if (next == null)
                    {
                        continue;
                    }

                    return next.TryGiveItem(checkDepositItem, GridHelpers.OppositeSide(outputSide));
                }
            }

            return false;
        }

        private void DepositItems()
        {
            if (Owner.Inventory == null)
            {
                return;
            }

            TryDeposit(null);
        }

        public void OnlyLetItemThroughSide(ItemType itemType, int sideOffset)
        {
            SideToOnlyAllowedItem[sideOffset] = itemType;
            ItemToOnlyAllowedSide[itemType] = sideOffset;
        }

        public void RemoveAllFilters()
        {
            SideToOnlyAllowedItem = new Dictionary<int, ItemType>();
            ItemToOnlyAllowedSide = new Dictionary<ItemType, int>();
        }
    }
}