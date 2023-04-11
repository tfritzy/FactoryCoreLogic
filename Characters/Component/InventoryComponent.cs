using System; // Needed in 4.7.1
using System.Linq;
using Newtonsoft.Json;

namespace Core
{
    public class InventoryComponent : Component
    {
        private Item?[] items;

        public override ComponentType Type => ComponentType.Inventory;

        public InventoryComponent(Entity owner, Item?[] items) : base(owner)
        {
            this.items = items;
        }

        public InventoryComponent(Entity owner, int size) : base(owner)
        {
            this.items = new Item?[size];
        }

        public int Size => items.Length;

        public bool CanAddItem(Item item)
        {
            return CanAddItem(item.Type, item.Quantity);
        }

        public bool CanAddItem(ItemType itemType, int quantity)
        {
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

        public void AddItem(Item item, int index)
        {
            if (index < 0 || index >= items.Length)
                return;

            Item? currentSlot = items[index];

            if (currentSlot == null)
            {
                items[index] = item;
                return;
            }

            if (currentSlot.Type == item.Type && currentSlot.Quantity < currentSlot.MaxStack)
            {
                int maxAddable = currentSlot.MaxStack - currentSlot.Quantity;
                int numToAdd = Math.Min(maxAddable, item.Quantity);

                currentSlot.AddToStack(numToAdd);
                item.RemoveFromStack(numToAdd);
            }
        }

        public void AddItem(Item item)
        {
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
                return;

            for (int i = 0; i < items.Length; i++)
            {
                if (items[i] == null)
                {
                    items[i] = item;
                    return;
                }
            }
        }

        public Item? GetItem(int index)
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

        public override Schema.Component ToSchema()
        {
            Schema.InventoryComponent schema = new Schema.InventoryComponent
            {
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
