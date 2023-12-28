using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Core
{
    public class SmeltingRecipe
    {
        public Dictionary<ItemType, uint> Inputs;
        public Dictionary<ItemType, uint> Outputs;
        public float TotalMassOfIngredients_Mg;
        public float AverageSpecificHeat_JPerMgPerC;
        public float HighestMeltingPoint_C;

        public SmeltingRecipe(
            Dictionary<ItemType, uint> inputs,
            Dictionary<ItemType, uint> outputs
        )
        {
            Inputs = inputs;
            Outputs = outputs;

            TotalMassOfIngredients_Mg = Inputs.Values.Sum((v) => v);
            AverageSpecificHeat_JPerMgPerC = Inputs.Keys.Average(
                (t) => Item.ItemProperties[t].SpecificHeat_JoulesPerMgPerDegreeCelsious ?? 0);
            HighestMeltingPoint_C = Inputs.Keys.Max((t) => Item.ItemProperties[t].MeltingPoint_Celsious ?? 0);
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
                        {ItemType.Chalcopyrite, 57_000_000}
                    },
                    outputs: new Dictionary<ItemType, uint>() {
                        {ItemType.CopperBar, 1},
                        {ItemType.IronSiliconSlag, 37_200_000}
                    }
                ),
            };
    }
}