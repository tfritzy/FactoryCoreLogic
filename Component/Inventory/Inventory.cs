using System; // Needed in 4.7.1
using System.Collections.Generic;
using System.Linq;
using Schema;

namespace Core
{
    public class Inventory : Component
    {
        public int Width { get; private set; }
        public int Height { get; private set; }
        private Item?[] items;
        public int Version { get; private set; }

        public override ComponentType Type => ComponentType.Inventory;

        public Inventory(Schema.Inventory schema, Entity owner) : base(owner)
        {
            this.Width = schema.Width;
            this.Height = schema.Height;
            this.items = schema.Items.Select(
                item => item.IsNull ?
                    null :
                    Item.FromSchema(item.Item)).ToArray();
        }

        public Inventory(Entity owner, Item?[] items, int width, int height) : base(owner)
        {
            this.Width = width;
            this.Height = height;
            this.items = items;
        }

        public Inventory(Entity owner, int width, int height) : base(owner)
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

        public virtual bool CanAddItem(ItemType itemType, ulong quantity)
        {
            if (Disabled)
                return false;

            ulong remainingUnplaced = quantity;
            for (int i = 0; i < items.Length; i++)
            {
                Item? currentSlot = items[i];

                if (currentSlot == null)
                    return true;

                if (currentSlot.Type == itemType && currentSlot.Quantity < currentSlot.MaxStack)
                {
                    ulong maxAddable = currentSlot.MaxStack - currentSlot.Quantity;
                    ulong numToAdd = Math.Min(maxAddable, remainingUnplaced);

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
        public virtual bool AddItem(Item item, int index)
        {
            if (Disabled)
                return false;

            if (index < 0 || index >= items.Length)
                throw new InvalidOperationException(
                    "Cannot add item to invalid index: " + index + ". Inventory size is " + items.Length + ".");

            Item? currentSlot = items[index];

            if (currentSlot == null)
            {
                Version++;
                items[index] = item;
                return true;
            }

            if (currentSlot.Type == item.Type && currentSlot.Quantity < currentSlot.MaxStack)
            {
                Version++;
                ulong maxAddable = currentSlot.MaxStack - currentSlot.Quantity;
                ulong numToAdd = Math.Min(maxAddable, item.Quantity);

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
        public virtual bool AddItem(Item item)
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
                    ulong maxAddable = currentSlot.MaxStack - currentSlot.Quantity;
                    ulong numToAdd = Math.Min(maxAddable, item.Quantity);

                    currentSlot.AddToStack(numToAdd);
                    item.RemoveFromStack(numToAdd);
                    Version++;
                }
            }

            if (item.Quantity == 0)
                return true;

            for (int i = 0; i < items.Length; i++)
            {
                if (items[i] == null)
                {
                    items[i] = item;
                    Version++;
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

        /// <summary>
        /// Returns the first item in the inventory that matches the given type.
        /// If no type is given, the first item in the inventory is returned.
        /// </summary>
        /// <param name="itemType">The type of item to find</param>
        /// <returns>The first item in the inventory that matches the given type</returns>
        public Item? FindItem(ItemType? itemType = null)
        {
            for (int i = 0; i < items.Length; i++)
            {
                if (itemType == null && items[i] != null)
                    return items[i];

                if (itemType != null && items[i]?.Type == itemType)
                    return items[i];
            }

            return null;
        }

        public Item? FindWhere(Func<Item?, bool> predicate)
        {
            for (int i = 0; i < items.Length; i++)
            {
                if (predicate(items[i]))
                    return items[i];
            }

            return null;
        }

        public ulong GetItemCount(ItemType itemType)
        {
            ulong count = 0;
            for (int i = 0; i < items.Length; i++)
            {
                if (items[i]?.Type == itemType)
                    count += items[i]?.Quantity ?? 0u;
            }

            return count;
        }

        public int NumOpenSlots()
        {
            int count = 0;
            for (int i = 0; i < items.Length; i++)
            {
                if (items[i] == null)
                    count++;
            }

            return count;
        }

        public void DecrementCountOf(int index, ulong quantity)
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
            {
                items[index] = null;
                Version++;
            }
        }

        public Item? Take1Quantity(ItemType? itemType = null)
        {
            if (Disabled)
                return null;

            for (int i = 0; i < items.Length; i++)
            {
                var item = items[i];
                if ((item != null && item.Type == itemType) || (itemType == null && item != null))
                {
                    DecrementCountOf(i, 1);
                    var newItem = Item.Create(item.Type);
                    newItem.SetQuantity(1);
                    return newItem;
                }
            }

            return null;
        }

        public void RemoveCount(ItemType itemType, ulong quantity)
        {
            if (Disabled)
                return;

            if (GetItemCount(itemType) < quantity)
                throw new InvalidOperationException("Cannot remove more items than are in the inventory.");

            ulong remainingToRemove = quantity;
            for (int i = 0; i < items.Length; i++)
            {
                if (items[i]?.Type == itemType)
                {
                    ulong numToRemove = Math.Min(items[i]?.Quantity ?? 0, remainingToRemove);
                    DecrementCountOf(i, numToRemove);
                    remainingToRemove -= numToRemove;

                    if (remainingToRemove <= 0)
                        return;
                }
            }
        }

        public void RemoveAll(ItemType itemType)
        {
            if (Disabled)
                return;

            for (int i = 0; i < items.Length; i++)
            {
                if (items[i]?.Type == itemType)
                {
                    items[i] = null;
                }
            }
        }

        public void TransferIndex(Inventory other, int sourceIndex)
        {
            if (Disabled)
                return;

            if (sourceIndex < 0 || sourceIndex >= items.Length)
                return;

            Item? item = items[sourceIndex];
            if (item == null)
                return;

            bool fullyAdded = other.AddItem(item);
            Version++;
            if (fullyAdded)
                items[sourceIndex] = null;
        }

        public void TransferIndex(Inventory other, int fromIndex, int toIndex)
        {
            if (Disabled)
                return;

            if (fromIndex < 0 || fromIndex >= items.Length)
                return;

            Item? item = items[fromIndex];
            if (item == null)
                return;

            bool fullyAdded = other.AddItem(item, toIndex);
            Version++;

            if (fullyAdded)
                items[fromIndex] = null;
        }

        public void TransferSingle(Inventory other, int fromIndex, int toIndex)
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
            Version++;

            if (fullyAdded)
                DecrementCountOf(fromIndex, 1);
        }

        public void BalanceBetween(List<int> indexes)
        {
            if (Disabled)
                return;

            Item? item = GetItemAt(indexes[0]);
            if (item == null)
                return;

            Version++;

            indexes = new List<int>(indexes);
            for (int i = 0; i < indexes.Count; i++)
            {
                Item? target = GetItemAt(indexes[i]);
                if (indexes[i] < 0 || indexes[i] >= items.Length ||
                    (target != null && target.Type != item.Type))
                {
                    indexes.RemoveAt(i);
                    i--;
                }
            }

            ulong totalQuantity = 0;
            for (int i = 0; i < indexes.Count; i++)
            {
                Item? currentItem = GetItemAt(indexes[i]);
                if (currentItem != null)
                    totalQuantity += currentItem.Quantity;
            }

            ulong numTargets = (ulong)indexes.Count;
            ulong quantityPerTarget = totalQuantity / (ulong)numTargets;
            ulong remainder = totalQuantity % (ulong)numTargets;

            for (ulong i = 0; i < numTargets; i++)
            {
                Item? currentItem = GetItemAt(indexes[(int)i]);

                if (currentItem == null)
                {
                    Item toAdd = Item.Create(item.Type);
                    toAdd.SetQuantity(0);

                    ulong quantity = quantityPerTarget;
                    if (i < remainder)
                        quantity++;
                    toAdd.SetQuantity(quantity);

                    AddItem(toAdd, indexes[(int)i]);
                }
                else
                {
                    ulong quantityToAdd = quantityPerTarget - currentItem.Quantity;
                    if (i < remainder)
                        quantityToAdd++;

                    currentItem.AddToStack(quantityToAdd);
                }
            }
        }

        public override OneofComponent ToSchema()
        {
            var schema = new OneofComponent
            {
                Inventory = new Schema.Inventory()
                {
                    Height = Height,
                    Width = Width,
                }
            };
            schema.Inventory.Items.AddRange(
                items.Select(
                    item => item != null ?
                        new MaybeNullItem { Item = item?.ToSchema() } :
                        new MaybeNullItem { IsNull = true }));
            return schema;
        }

        public Item?[] GetCopyOfItems()
        {
            return items.Select(item => item).ToArray();
        }
    }
}
