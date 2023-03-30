using Newtonsoft.Json;

namespace FactoryCore
{
    [JsonObject(MemberSerialization.OptIn)]
    public class HarvestableComponent : Component
    {
        public ItemType ProducedItemType { get; private set; }
        public int MaxHarvestItems { get; private set; }
        public int RemainingItems { get; private set; }
        public HarvestableType HarvestableType { get; private set; }

        [JsonConstructor]
        public HarvestableComponent(Entity owner, HarvestableType type, int remainingItems) : base(owner)
        {
            this.HarvestableType = type;
            this.RemainingItems = remainingItems;
        }

        public override ComponentType Type => ComponentType.Harvestable;

        public HarvestableComponent(Entity owner, ItemType produces, int maxItems, HarvestableType harvestableType) : base(owner)
        {
            this.ProducedItemType = produces;
            this.MaxHarvestItems = maxItems;
            this.RemainingItems = maxItems;
            this.HarvestableType = harvestableType;
        }

        public Item BuildHarvestedItem() => Item.Create(ProducedItemType);

        public Item? Harvest()
        {
            if (RemainingItems > 0)
            {
                RemainingItems--;
                return BuildHarvestedItem();
            }
            else
            {
                return null;
            }
        }
    }
}