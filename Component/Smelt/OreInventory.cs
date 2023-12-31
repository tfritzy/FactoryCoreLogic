namespace Core
{
    public class OreInventory : Inventory
    {
        public OreInventory(Entity owner, int width, int height) : base(owner, width, height)
        {
        }

        public override bool CanAddItem(ItemType itemType, ulong quantity)
        {
            if (!SmeltingRecipes.RecipeIngredients.Contains(itemType))
            {
                return false;
            }

            return base.CanAddItem(itemType, quantity);
        }

        public override bool AddItem(Item item)
        {
            if (!SmeltingRecipes.RecipeIngredients.Contains(item.Type))
            {
                return false;
            }

            return base.AddItem(item);
        }

        public override bool AddItem(Item item, int index)
        {
            if (!SmeltingRecipes.RecipeIngredients.Contains(item.Type))
            {
                return false;
            }

            return base.AddItem(item, index);
        }
    }
}