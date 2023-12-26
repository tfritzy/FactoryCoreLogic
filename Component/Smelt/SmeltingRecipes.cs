using System.Collections.Generic;
using System.ComponentModel;

namespace Core
{
    public class SmeltingRecipe
    {
        public Dictionary<ItemType, int> Inputs;
        public Dictionary<ItemType, int> Outputs;

        public SmeltingRecipe(
            Dictionary<ItemType, int> inputs,
            Dictionary<ItemType, int> outputs
        )
        {
            Inputs = inputs;
            Outputs = outputs;
        }
    }

    public static class SmeltingRecipes
    {
        private static HashSet<ItemType>? recipeIngredients;
        public static HashSet<ItemType> RecipeIngredients
        {
            get
            {
                if (recipeIngredients == null)
                {
                    recipeIngredients = new HashSet<ItemType>();
                    foreach (SmeltingRecipe recipe in Recipes)
                    {
                        foreach (ItemType inputType in recipe.Inputs.Keys)
                        {
                            recipeIngredients.Add(inputType);
                        }
                    }
                }

                return recipeIngredients;
            }
        }

        public static readonly List<SmeltingRecipe> Recipes =
            new List<SmeltingRecipe>{
                new SmeltingRecipe(
                    inputs: new Dictionary<ItemType, int>() {
                        {ItemType.Magnetite, 1}
                    },
                    outputs: new Dictionary<ItemType, int>() {
                        {ItemType.IronBar, 1}
                    }
                ),
                new SmeltingRecipe(
                    inputs: new Dictionary<ItemType, int>() {
                        {ItemType.Chalcopyrite, 2}
                    },
                    outputs: new Dictionary<ItemType, int>() {
                        {ItemType.IronBar, 1},
                        {ItemType.CopperBar, 1}
                    }
                ),
            };
    }
}