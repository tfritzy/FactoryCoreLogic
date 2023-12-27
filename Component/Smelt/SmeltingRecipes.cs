using System.Collections.Generic;
using System.ComponentModel;

namespace Core
{
    public class SmeltingRecipe
    {
        public Dictionary<ItemType, uint> Inputs;
        public Dictionary<ItemType, uint> Outputs;

        public SmeltingRecipe(
            Dictionary<ItemType, uint> inputs,
            Dictionary<ItemType, uint> outputs
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
                    inputs: new Dictionary<ItemType, uint>() {
                        {ItemType.Magnetite, 1}
                    },
                    outputs: new Dictionary<ItemType, uint>() {
                        {ItemType.IronBar, 1}
                    }
                ),
                new SmeltingRecipe(
                    inputs: new Dictionary<ItemType, uint>() {
                        {ItemType.Chalcopyrite, 1}
                    },
                    outputs: new Dictionary<ItemType, uint>() {
                        {ItemType.IronSiliconSlag, 1},
                        {ItemType.CopperBar, 1}
                    }
                ),
            };
    }
}