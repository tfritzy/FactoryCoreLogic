using Newtonsoft.Json;

namespace FactoryCore
{
    [JsonObject(MemberSerialization.OptIn)]
    public class HarvestableCell : Cell
    {
        [JsonProperty("producedItemType")]
        public ItemType ProducedItemType { get; }

        [JsonProperty("maxHarvestItems")]
        public int MaxHarvestItems { get; }

        [JsonProperty("remainingItems")]
        public int RemainingItems { get; private set; }

        [JsonProperty("harvestableType")]
        public HarvestableType HarvestableType { get; private set; }

        [JsonConstructor]
        public HarvestableCell(EntityComponent owner) : base(owner)
        {
        }

        public override CellType Type => CellType.Harvestable;


        public HarvestableCell(EntityComponent owner, ItemType produces, int maxItems, HarvestableType harvestableType) : base(owner)
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