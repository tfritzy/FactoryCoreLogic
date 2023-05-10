using System.Collections.Generic;

namespace Core
{
    public abstract class Worksite : Component
    {
        public override ComponentType Type => ComponentType.Worksite;
        public abstract int MaxEmployable { get; }
        public abstract List<Harvestable> RefreshEligible();
        private List<Harvestable> eligibleHarvestables = new List<Harvestable>();

        public Worksite(Entity owner) : base(owner) { }

        public int Employed => this.World.Villagers.FindAll(
            v => World.GetCharacter(v).GetComponent<VillagerBehavior>().PlaceOfEmployment == this).Count;

        public override Schema.Component ToSchema()
        {
            throw new System.NotImplementedException();
        }

        public Harvestable GetNextHarvestable()
        {
            if (this.eligibleHarvestables.Count == 0)
            {
                this.eligibleHarvestables = this.RefreshEligible();
            }

            return this.eligibleHarvestables[0];
        }

        public void IncrementEmployed()
        {
            if (Employed >= this.MaxEmployable)
            {
                throw new System.InvalidOperationException("Max employed reached");
            }

            var unemployedVillagers =
                this.World.Villagers.FindAll(
                    v => World.GetCharacter(v)?.GetComponent<VillagerBehavior>().PlaceOfEmployment == null);

            if (unemployedVillagers.Count == 0)
            {
                throw new System.InvalidOperationException("No unemployed villagers");
            }

            Villager villager = (Villager)this.World.GetCharacter(unemployedVillagers[0]);
            villager.Behavior.SetPlaceOfEmployment(this);
        }

        public void DecrementEmployed()
        {
            var employedVillagers =
                this.World.Villagers.FindAll(
                    v => World.GetCharacter(v).GetComponent<VillagerBehavior>().PlaceOfEmployment == this);

            if (employedVillagers.Count <= 0)
            {
                throw new System.InvalidOperationException("No employed villagers");
            }

            Villager gettingTheCan = (Villager)this.World.GetCharacter(employedVillagers[0]);
            gettingTheCan.Behavior.SetPlaceOfEmployment(null);
        }
    }
}