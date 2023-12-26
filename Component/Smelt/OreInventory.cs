namespace Core
{
    public class OreInventory : Inventory
    {
        public OreInventory(Entity owner, int width, int height) : base(owner, width, height)
        {
        }

        public override bool CanAddItem(ItemType itemType, int quantity)
        {
            if (!SmeltingRecipes.RecipeIngredients.Contains(itemType))
            {
                return false;
            }

            return base.CanAddItem(itemType, quantity);
        }
    }
}