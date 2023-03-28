using System.Collections.Generic; // Needed in 4.7.1
using Newtonsoft.Json;

namespace FactoryCore
{
    public class HarvestCell : Cell
    {
        [JsonProperty("harvestRateSeconds")]
        public Dictionary<HarvestableType, float> HarvestRateSeconds { get; private set; }

        [JsonProperty("type")]
        public override CellType Type => CellType.Harvest;

        [JsonProperty("harvestTargetId")]
        public ulong? HarvestTargetId { get; private set; }

        [JsonProperty("targetHarvestPoint")]
        public Point3Int? TargetHarvestPoint { get; private set; }

        private float timeUntilHarvest;

        private Harvestable? target;
        private Harvestable? GetTarget()
        {
            if (target != null)
            {
                if (HarvestTargetId != null)
                {
                    if (!(target.Owner is Character) || target.Owner.Id != HarvestTargetId)
                    {
                        target = null;
                    }
                }
                else if (TargetHarvestPoint != null)
                {
                    if (!(target.Owner is Hex) || target.Owner.Context.World.GetHex(TargetHarvestPoint.Value) == null)
                    {
                        target = null;
                    }
                }
            }

            if (HarvestTargetId != null)
            {
                if (this.World.TryGetCharacter(HarvestTargetId.Value, out var targetChar))
                {
                    return targetChar?.GetCell<Harvestable>();
                }
                else
                {
                    return null;
                }
            }
            else if (TargetHarvestPoint != null)
            {
                return this.World.GetHex(TargetHarvestPoint.Value)?.GetCell<Harvestable>();
            }
            else
            {
                return null;
            }
        }

        public HarvestCell(Character owner, Dictionary<HarvestableType, float> harvestRateSeconds) : base(owner)
        {
            this.HarvestRateSeconds = harvestRateSeconds;
            this.timeUntilHarvest = float.MaxValue;
        }

        public override void Tick(float deltaTime)
        {
            var target = GetTarget();
            if (target == null || !HarvestRateSeconds.ContainsKey(target.HarvestableType))
            {
                return;
            }

            if (Owner.Inventory == null)
            {
                return;
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
            }
        }

        public void SetTarget(Point3Int targetHex)
        {
            this.TargetHarvestPoint = targetHex;
            this.HarvestTargetId = null;

            Harvestable? target = GetTarget();
            if (target == null || !HarvestRateSeconds.ContainsKey(target.HarvestableType))
            {
                return;
            }

            this.timeUntilHarvest = HarvestRateSeconds[target.HarvestableType];
        }

        public void SetTarget(ulong targetCharacterId)
        {
            this.HarvestTargetId = targetCharacterId;
            this.TargetHarvestPoint = null;

            Harvestable? target = GetTarget();
            if (target == null || !HarvestRateSeconds.ContainsKey(target.HarvestableType))
            {
                return;
            }

            this.timeUntilHarvest = HarvestRateSeconds[target.HarvestableType];
        }
    }
}