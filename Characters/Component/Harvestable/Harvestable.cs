using System.Collections.Generic;
using Newtonsoft.Json;

namespace Core
{
    public class Harvestable : Component
    {
        public ItemType ProducedItemType { get; private set; }
        public int MaxHarvestItems { get; private set; }
        public int RemainingItems { get; set; }
        public HarvestableType HarvestableType { get; private set; }
        public override ComponentType Type => ComponentType.Harvestable;
        public Item BuildHarvestedItem() => Item.Create(ProducedItemType);
        public bool IsDepleted => RemainingItems == 0;

        public Harvestable(Entity owner, HarvestableType type) : base(owner)
        {
            this.HarvestableType = type;
            this.ProducedItemType = HarvestableTypeToItem[type].ProducedItemType;
            this.MaxHarvestItems = HarvestableTypeToItem[type].MaxHarvestItems;
            this.RemainingItems = this.MaxHarvestItems;
        }

        private struct HarvestableStats
        {
            public ItemType ProducedItemType;
            public int MaxHarvestItems;

            public HarvestableStats(ItemType itemType, int maxHarvestItems)
            {
                ProducedItemType = itemType;
                MaxHarvestItems = maxHarvestItems;
            }
        }

        private Dictionary<HarvestableType, HarvestableStats> HarvestableTypeToItem = new Dictionary<HarvestableType, HarvestableStats>()
        {
            { HarvestableType.Tree, new HarvestableStats(ItemType.Wood, 16) },
            { HarvestableType.StoneHex, new HarvestableStats(ItemType.Stone, 8) },
            { HarvestableType.DirtHex, new HarvestableStats(ItemType.Dirt, 8) },
        };

        public Item? Harvest()
        {
            if (RemainingItems > 0)
            {
                RemainingItems--;

                if (RemainingItems == 0)
                {
                    DestroyOwner();
                }

                return BuildHarvestedItem();
            }
            else
            {
                return null;
            }
        }

        public override Schema.Component ToSchema()
        {
            return new Schema.Harvestable()
            {
                RemainingItems = RemainingItems,
                HarvestableType = HarvestableType
            };
        }

        private void DestroyOwner()
        {
            if (this.Owner is Character)
            {
                ((Character)this.Owner).Destroy();
            }
            else if (this.Owner is Hex)
            {
                ((Hex)this.Owner).Destroy();
            }
        }
    }
}