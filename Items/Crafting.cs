namespace Core
{
    public static class Crafting
    {
        public static void CraftItem(ItemType type, InventoryComponent inventory)
        {
            Item item = Item.Create(type);
            if (item.Recipe == null)
            {
                throw new System.InvalidOperationException("Item does not have a recipe");
            }

            if (!inventory.CanAddItem(item))
            {
                if (!CraftingItemOpensSlot(item, inventory))
                    throw new System.InvalidOperationException("Not enough space in inventory");
            }

            foreach (var (ingredientType, quantity) in item.Recipe)
            {
                int ingredientCount = inventory.GetItemCount(ingredientType);

                if (ingredientCount < quantity)
                {
                    throw new System.InvalidOperationException("Not enough ingredients");
                }
            }

            foreach (var (ingredientType, quantity) in item.Recipe)
            {
                inventory.RemoveCount(ingredientType, quantity);
            }

            inventory.AddItem(item);
        }

        private static bool CraftingItemOpensSlot(Item item, InventoryComponent inventory)
        {
            if (item.Recipe == null)
                return false;

            foreach (var (ingredientType, quantity) in item.Recipe)
            {
                if (RemovingItemOpensSlot(ingredientType, quantity, inventory))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool RemovingItemOpensSlot(ItemType itemType, int quantity, InventoryComponent inventory)
        {
            int remainingToRemove = quantity;
            for (int i = 0; i < inventory.Size; i++)
            {
                if (inventory.GetItem(i)?.Type == itemType)
                {
                    int numToRemove = System.Math.Min(remainingToRemove, inventory.GetItem(i)?.Quantity ?? 0);
                    remainingToRemove -= numToRemove;

                    if (numToRemove == inventory.GetItem(i)?.Quantity)
                        return true;
                }
            }

            return false;
        }
    }
}