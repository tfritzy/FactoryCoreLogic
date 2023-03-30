using System; // Needed in 4.7.1
using Newtonsoft.Json;

namespace FactoryCore
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
    }
}
