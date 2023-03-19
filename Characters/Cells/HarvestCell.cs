namespace FactoryCore
{
    public class HarvestCell : Cell
    {
        public override CellType Type => CellType.Harvest;
        public Dictionary<HarvestableType, float> HarvestRateSeconds { get; private set; }
        private float timeUntilHarvest;
        private Harvestable? target;

        public HarvestCell(Character owner, Dictionary<HarvestableType, float> harvestRateSeconds) : base(owner)
        {
            this.HarvestRateSeconds = harvestRateSeconds;
            this.timeUntilHarvest = float.MaxValue;
        }

        public override void Tick(float deltaTime)
        {
            if (target == null || !HarvestRateSeconds.ContainsKey(target.HarvestableType))
            {
                return;
            }

            timeUntilHarvest -= deltaTime;
            if (timeUntilHarvest <= 0)
            {
                timeUntilHarvest = HarvestRateSeconds[this.target.HarvestableType];
                target.Harvest();
            }
        }

        public void SetTarget(Harvestable target)
        {
            this.target = target;
            this.timeUntilHarvest = HarvestRateSeconds[target.HarvestableType];
        }
    }
}