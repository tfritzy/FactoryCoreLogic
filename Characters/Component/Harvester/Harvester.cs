using System.Collections.Generic; // Needed in 4.7.1
using Newtonsoft.Json;

namespace Core
{
    public class Harvester : Component
    {
        public Dictionary<HarvestableType, float> HarvestRateSeconds { get; set; }
        public override ComponentType Type => ComponentType.Harvester;
        private float? timeUntilHarvest;
        private Harvestable? target;

        public Harvester(Entity owner) : base(owner)
        {
            this.HarvestRateSeconds = new Dictionary<HarvestableType, float>();
            this.timeUntilHarvest = null;
        }

        public override void Tick(float deltaTime)
        {
            if (Disabled)
            {
                return;
            }

            if (target == null || !HarvestRateSeconds.ContainsKey(target.HarvestableType))
            {
                return;
            }

            if (Owner.Inventory == null)
            {
                return;
            }

            if (timeUntilHarvest == null)
            {
                timeUntilHarvest = HarvestRateSeconds[target.HarvestableType];
            }

            timeUntilHarvest -= deltaTime;
            while (timeUntilHarvest <= 0)
            {
                timeUntilHarvest += HarvestRateSeconds[target.HarvestableType];

                if (Owner.Inventory.CanAddItem(target.ProducedItemType, 1))
                {
                    var item = target.Harvest();

                    if (item != null)
                    {
                        Owner.Inventory.AddItem(item);
                    }
                }

                if (target.IsDepleted) 
                {
                    timeUntilHarvest = null;
                    return;
                }
            }
        }

        public void SetTarget(Harvestable target)
        {
            this.target = target;
            if (!HarvestRateSeconds.ContainsKey(target.HarvestableType))
            {
                return;
            }

            this.timeUntilHarvest = HarvestRateSeconds[target.HarvestableType];
        }

        public override Schema.Component ToSchema()
        {
            return new Schema.Harvest()
            {
                HarvestRateSeconds = this.HarvestRateSeconds,
            };
        }

        public void SetTimeUntilHarvest(float timeUntilHarvest)
        {
            this.timeUntilHarvest = timeUntilHarvest;
        }
    }
}