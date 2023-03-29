using Newtonsoft.Json;

namespace FactoryCore
{
    [JsonObject(MemberSerialization.OptIn)]
    public class HarvestableComponent : Component
    {
        [JsonProperty("producedItemType")]
        public ItemType ProducedItemType { get; private set; }

        [JsonProperty("maxHarvestItems")]
        public int MaxHarvestItems { get; private set; }

        [JsonProperty("remainingItems")]
        public int RemainingItems { get; private set; }

        [JsonProperty("harvestableType")]
        public HarvestableType HarvestableType { get; private set; }

        [JsonConstructor]
        public HarvestableComponent(Entity owner) : base(owner)
        {
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