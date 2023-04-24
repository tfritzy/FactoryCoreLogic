using System; // Needed in 4.7.1
using System.Linq;
using Newtonsoft.Json;

namespace Core
{
    public class InventoryComponent : Component
    {
        public int Width { get; private set; }
        public int Height { get; private set; }
        private Item?[] items;

        public override ComponentType Type => ComponentType.Inventory;

        public InventoryComponent(Entity owner, Item?[] items, int width, int height) : base(owner)
        {
            this.Width = width;
            this.Height = height;
            this.items = items;
        }

        public InventoryComponent(Entity owner, int width, int height) : base(owner)
        {
            this.Width = width;
            this.Height = height;
            this.items = new Item?[width * height];
        }

        public int Size => Width * Height;

        public bool CanAddItem(Item item)
        {
            return CanAddItem(item.Type, item.Quantity);
        }

        public bool CanAddItem(ItemType itemType, int quantity)
        {
            if (Disabled)
                return false;

            int remainingUnplaced = quantity;
            for (int i = 0; i < items.Length; i++)
            {
                Item? currentSlot = items[i];

                if (currentSlot == null)
                    return true;

                if (currentSlot.Type == itemType && currentSlot.Quantity < currentSlot.MaxStack)
                {
                    int maxAddable = currentSlot.MaxStack - currentSlot.Quantity;
                    int numToAdd = Math.Min(maxAddable, remainingUnplaced);

                    remainingUnplaced -= numToAdd;

                    if (remainingUnplaced <= 0)
                        return true;
                }
            }

            return remainingUnplaced <= 0;
        }

        /// <summary>
        /// Adds the given item to the inventory at the given index. If the slot is
        /// already occupied and the item is the same type as the item in the slot
        /// the item will be added to the stack. If the item cannot be fully added 
        /// to the inventory, this method will return false.
        /// </summary>
        /// <param name="item">The item to add.</param>
        /// <param name="index">THe index the item will occupy</param>
        /// <returns>true if item was fully added, false otherwise</returns>
        public bool AddItem(Item item, int index)
        {
            if (Disabled)
                return false;

            if (index < 0 || index >= items.Length)
                return false;

            Item? currentSlot = items[index];

            if (currentSlot == null)
            {
                items[index] = item;
                return true;
            }

            if (currentSlot.Type == item.Type && currentSlot.Quantity < currentSlot.MaxStack)
            {
                int maxAddable = currentSlot.MaxStack - currentSlot.Quantity;
                int numToAdd = Math.Min(maxAddable, item.Quantity);

                currentSlot.AddToStack(numToAdd);
                item.RemoveFromStack(numToAdd);

                return item.Quantity == 0;
            }

            return false;
        }

        /// <summary>
        /// Adds the given item to this inventory. If there are stacks of the item
        /// already present filling them will be prioritized, and then empty slots.
        /// 
        /// If the item cannot be fully added the method will return false.
        /// </summary>
        /// <param name="item">The item to add to the inventory</param>
        /// <returns>true if item was fully added, false otherwise</returns>
        public bool AddItem(Item item)
        {
            if (Disabled)
                return false;

            for (int i = 0; i < items.Length; i++)
            {
                Item? currentSlot = items[i];

                if (currentSlot == null)
                    continue;

                if (currentSlot.Type == item.Type && currentSlot.Quantity < currentSlot.MaxStack)
                {
                    int maxAddable = currentSlot.MaxStack - currentSlot.Quantity;
                    int numToAdd = Math.Min(maxAddable, item.Quantity);

                    currentSlot.AddToStack(numToAdd);
                    item.RemoveFromStack(numToAdd);
                }
            }

            if (item.Quantity == 0)
                return true;

            for (int i = 0; i < items.Length; i++)
            {
                if (items[i] == null)
                {
                    items[i] = item;
                    return true;
                }
            }

            return false;
        }

        public Item? GetItemAt(int index)
        {
            if (index < 0 || index >= items.Length)
                return null;

            return items[index];
        }

        public Item? FindItem(ItemType itemType)
        {
            for (int i = 0; i < items.Length; i++)
            {
                if (items[i]?.Type == itemType)
                    return items[i];
            }

            return null;
        }

        public int GetItemCount(ItemType itemType)
        {
            int count = 0;
            for (int i = 0; i < items.Length; i++)
            {
                if (items[i]?.Type == itemType)
                    count += items[i]?.Quantity ?? 0;
            }

            return count;
        }

        public void DecrementCountOf(int index, int quantity)
        {
            if (Disabled)
                return;

            if (index < 0 || index >= items.Length)
                return;

            Item? item = items[index];
            if (item == null)
                return;

            if (quantity < 0)
                throw new InvalidOperationException("Cannot remove a negative quantity of items.");

            if (quantity > item.Quantity)
                throw new InvalidOperationException("Cannot remove more items than are in the stack.");

            item.RemoveFromStack(quantity);
            if (item.Quantity <= 0)
                items[index] = null;
        }

        public void RemoveCount(ItemType itemType, int quantity)
        {
            if (Disabled)
                return;

            if (GetItemCount(itemType) < quantity)
                throw new InvalidOperationException("Cannot remove more items than are in the inventory.");

            int remainingToRemove = quantity;
            for (int i = 0; i < items.Length; i++)
            {
                if (items[i]?.Type == itemType)
                {
                    int numToRemove = Math.Min(items[i]?.Quantity ?? 0, remainingToRemove);
                    DecrementCountOf(i, numToRemove);
                    remainingToRemove -= numToRemove;

                    if (remainingToRemove <= 0)
                        return;
                }
            }
        }

        public void TransferIndex(InventoryComponent other, int sourceIndex)
        {
            if (Disabled)
                return;

            if (sourceIndex < 0 || sourceIndex >= items.Length)
                return;

            Item? item = items[sourceIndex];
            if (item == null)
                return;

            bool fullyAdded = other.AddItem(item);

            if (fullyAdded)
                items[sourceIndex] = null;
        }

        public void TransferIndex(InventoryComponent other, int fromIndex, int toIndex)
        {
            if (Disabled)
                return;

            if (fromIndex < 0 || fromIndex >= items.Length)
                return;

            Item? item = items[fromIndex];
            if (item == null)
                return;

            bool fullyAdded = other.AddItem(item, toIndex);

            if (fullyAdded)
                items[fromIndex] = null;
        }

        public void TransferSingle(InventoryComponent other, int fromIndex, int toIndex)
        {
            if (Disabled)
                return;

            if (fromIndex < 0 || fromIndex >= items.Length)
                return;

            Item? item = items[fromIndex];
            if (item == null || item.Quantity < 1)
                return;

            Item toAdd = Item.Create(item.Type);

            bool fullyAdded = other.AddItem(toAdd, toIndex);

            if (fullyAdded)
                DecrementCountOf(fromIndex, 1);
        }

        public override Schema.Component ToSchema()
        {
            Schema.InventoryComponent schema = new Schema.InventoryComponent
            {
                Height = this.Height,
                Width = this.Width,
                Items = this.items.Select(item => item?.ToSchema()).ToArray(),
            };

            for (int i = 0; i < items.Length; i++)
            {
                Item? item = items[i];
                if (item != null)
                    schema.Items[i] = item.ToSchema();
            }

            return schema;
        }
    }
}
