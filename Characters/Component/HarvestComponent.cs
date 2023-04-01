using System.Collections.Generic; // Needed in 4.7.1
using Newtonsoft.Json;

namespace Core
{
    public class HarvestComponent : Component
    {
        [JsonProperty("harvestRateSeconds")]
        public Dictionary<HarvestableType, float> HarvestRateSeconds { get; set; }

        [JsonProperty("type")]
        public override ComponentType Type => ComponentType.Harvest;

        [JsonProperty("harvestTargetId")]
        public ulong? HarvestTargetId { get; private set; }

        [JsonProperty("targetHarvestPoint")]
        public Point3Int? TargetHarvestPoint { get; private set; }

        private float timeUntilHarvest;

        private HarvestableComponent? target;
        private HarvestableComponent? GetTarget()
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
                    return targetChar?.GetComponent<HarvestableComponent>();
                }
                else
                {
                    return null;
                }
            }
            else if (TargetHarvestPoint != null)
            {
                return this.World.GetHex(TargetHarvestPoint.Value)?.GetComponent<HarvestableComponent>();
            }
            else
            {
                return null;
            }
        }

        public HarvestComponent(Entity owner) : base(owner)
        {
            this.HarvestRateSeconds = new Dictionary<HarvestableType, float>();
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

            HarvestableComponent? target = GetTarget();
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

            HarvestableComponent? target = GetTarget();
            if (target == null || !HarvestRateSeconds.ContainsKey(target.HarvestableType))
            {
                return;
            }

            this.timeUntilHarvest = HarvestRateSeconds[target.HarvestableType];
        }
    }
}