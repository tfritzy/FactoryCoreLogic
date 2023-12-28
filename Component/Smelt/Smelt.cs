using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Core
{
    // This thing
    //   - Accepts ore of dynamic type
    //   - Outputs ingot of the ore type
    //   - Can make alloys by having the right ores in its inventory
    //   - Does not accept non-ore items
    //   - Takes a dynamic amount of time te smelt, based on heat and ore type.
    //   - Can craft variants that smelt faster
    //   - Ingots are measured in kg. Smelting an ore type produces 
    //     ingots of size according to the ratio of elements.
    //   - Requires fuel. Some smelter types are powered by electricity and some
    //     by combustion. These two methods have different efficiencies, since induction
    //     transfers energy more efficiently than combustion.
    //   - Can intake fuel on a separate line, or through the main line.
    //   - Fuel is stored in a dedicated slot in the inventory.
    //   - Stops smelting if the output conveyor is full. This is awkward when the ore
    //   - produces multiple ingot types, since the conveyor accepts the ingots one at a time.
    //     maybe we can wait until the conveyor can accept at point 0, and then insert the others behind it.
    //   - Consumes fuel even when not smelting. This is to keep the smelter hot.
    //   - Combustion smelters cool down over time, and fuel is consumed when it gets too cold.
    //   - Induction smelters do not cool down, and use electricity in bursts.

    public class Smelt : Component
    {
        public float CombustionEfficiency { get; private set; }
        public float MassKg { get; private set; }
        public override ComponentType Type => ComponentType.Smelt;
        public Building BuildingOwner => (Building)Owner;
        public const float TempRatioRequiredToSmelt = 1.2f;

        public float SmeltingItemTemperature_C { get; private set; }
        private SmeltingRecipe? recipeBeingSmelted;
        Queue<Item> smeltedItems = new Queue<Item>();

        public Smelt(Entity owner) : base(owner)
        {
            SmeltingItemTemperature_C = Owner.Context.World.OutsideAirTemperature_C;
        }

        public override void Tick(float deltaTime)
        {
            base.Tick(deltaTime);
            SmeltOres(deltaTime);
        }

        private void SmeltOres(float deltaTime)
        {
            if (BuildingOwner.FuelInventory == null ||
                Owner.Inventory == null ||
                BuildingOwner.OreInventory == null)
            {
                return;
            }

            while (smeltedItems.Count > 0 && Owner.Inventory.CanAddItem(smeltedItems.First()))
            {
                Owner.Inventory.AddItem(smeltedItems.Dequeue());
            }

            if (smeltedItems.Count > 0)
            {
                return;
            }

            Item? fuel = BuildingOwner.FuelInventory.FindItem();
            if (fuel == null || fuel.Combustion == null)
            {
                return;
            }

            recipeBeingSmelted ??= GetCompleteSmeltingRecipe();

            if (recipeBeingSmelted == null)
            {
                return;
            }

            if (!StillHasIngredientsOfSmeltingRecipe())
            {
                recipeBeingSmelted = null;
                SmeltingItemTemperature_C = Owner.World.OutsideAirTemperature_C;
                return;
            }

            uint amountOfFuelCombusted_mg = (uint)(fuel.Combustion.Value.BurnRateMilligramPerSecond * deltaTime);
            amountOfFuelCombusted_mg =
                Math.Min(
                    BuildingOwner.FuelInventory.GetItemCount(fuel.Type),
                    amountOfFuelCombusted_mg);
            BuildingOwner.FuelInventory.RemoveCount(fuel.Type, amountOfFuelCombusted_mg);
            float energyAdded_Joules =
                amountOfFuelCombusted_mg *
                fuel.Combustion.Value.CalorificValue_JoulesPerMg
                * CombustionEfficiency;

            float temperatureChange_C =
                energyAdded_Joules /
                recipeBeingSmelted.AverageSpecificHeat_JPerMgPerC /
                recipeBeingSmelted.TotalMassOfIngredients_Mg;
            SmeltingItemTemperature_C += temperatureChange_C;

            if (SmeltingItemTemperature_C > recipeBeingSmelted.HighestMeltingPoint_C * TempRatioRequiredToSmelt)
            {
                foreach (ItemType type in recipeBeingSmelted.Inputs.Keys)
                {
                    BuildingOwner.OreInventory.RemoveCount(type, recipeBeingSmelted.Inputs[type]);
                }

                foreach (ItemType type in recipeBeingSmelted.Outputs.Keys)
                {
                    Item item = Item.Create(type);
                    item.SetQuantity(recipeBeingSmelted.Outputs[type]);

                    if (Owner.Inventory.CanAddItem(item))
                    {
                        Owner.Inventory.AddItem(item);
                    }
                    else
                    {
                        smeltedItems.Enqueue(item);
                    }
                }

                recipeBeingSmelted = null;
                SmeltingItemTemperature_C = Owner.Context.World.OutsideAirTemperature_C;
            }
        }

        private SmeltingRecipe? GetCompleteSmeltingRecipe()
        {
            if (BuildingOwner.OreInventory == null)
            {
                return null;
            }

            foreach (SmeltingRecipe recipe in SmeltingRecipes.Recipes)
            {
                bool hasEntireRecipe = true;
                foreach (ItemType type in recipe.Inputs.Keys)
                {
                    if (BuildingOwner.OreInventory.GetItemCount(type) < recipe.Inputs[type])
                    {
                        hasEntireRecipe = false;
                    }
                }

                if (hasEntireRecipe)
                {
                    return recipe;
                }
            }

            return null;
        }

        private bool StillHasIngredientsOfSmeltingRecipe()
        {
            if (recipeBeingSmelted == null)
            {
                return true;
            }

            foreach (ItemType type in recipeBeingSmelted.Inputs.Keys)
            {
                if (BuildingOwner.OreInventory?.GetItemCount(type) < recipeBeingSmelted.Inputs[type])
                {
                    return false;
                }
            }

            return true;
        }

        public void SetConstants(
            float combustionEfficiency)
        {
            CombustionEfficiency = combustionEfficiency;
        }

        public override Schema.Component ToSchema()
        {
            return new Schema.Smelt() { };
        }

        public static Func<Item, Inventory?> GetItemDirectionFunction(Building owner)
        {
            return (item) =>
            {
                if (item.Combustion != null)
                {
                    return owner.FuelInventory;
                }
                else if (SmeltingRecipes.RecipeIngredients.Contains(item.Type))
                {
                    return owner.OreInventory;
                }
                else
                {
                    return null;
                }
            };
        }
    }
}