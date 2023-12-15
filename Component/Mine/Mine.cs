using System;
using System.Collections.Generic;
using NoiseTest;

namespace Core
{
    public class Mine : Component
    {
        public override ComponentType Type => ComponentType.Mine;
        public const float CollectionTime = 5f;
        public ItemType UpcomingItemType { get; private set; }

        private float collectionTimeRemaining;
        public readonly Dictionary<ItemType, float> ResourceWeights;

        public Mine(Entity owner) : base(owner)
        {
            collectionTimeRemaining = CollectionTime;
            ResourceWeights = GetResourceWeights(0, (Point2Int)((Building)owner).GridPosition);
            UpcomingItemType = UpcomingItemType = GetRandomResource();
        }

        public override void Tick(float deltaTime)
        {
            base.Tick(deltaTime);

            collectionTimeRemaining -= deltaTime;
            if (collectionTimeRemaining <= 0)
            {
                if (Owner?.Inventory != null && Owner.Inventory.CanAddItem(UpcomingItemType, 1))
                {
                    Item item = Item.Create(UpcomingItemType);
                    Owner.Inventory.AddItem(item);

                    collectionTimeRemaining = CollectionTime;
                    UpcomingItemType = GetRandomResource();
                }
            }
        }

        public override Schema.Component ToSchema()
        {
            return new Schema.Mine();
        }

        private ItemType GetRandomResource()
        {
            Random random = new();
            float value = (float)random.NextDouble();
            float totalWeight = 0;
            foreach (var weight in ResourceWeights)
            {
                totalWeight += weight.Value;
                if (value <= totalWeight)
                {
                    return weight.Key;
                }
            }

            return ItemType.Stone;
        }

        private const float MaxOrePercent = .5f;
        private static readonly List<ItemType> OreTypes = new() { ItemType.Coal, ItemType.IronOre };
        public static Dictionary<ItemType, float> GetResourceWeights(int seed, Point2Int location)
        {
            Dictionary<ItemType, float> weights = new();
            OpenSimplexNoise noise = new OpenSimplexNoise();
            List<float> percents = new();
            float sum = 0;
            for (int i = 0; i < OreTypes.Count; i++)
            {
                float eval = (float)noise.Evaluate(location.x / 100f, location.y / 100f, i);
                eval = (eval + 1) / 2;
                sum += eval;
                percents.Add(eval);
            }

            float updatedSum = 0;
            if (sum > MaxOrePercent)
            {
                float ratio = MaxOrePercent / sum;
                for (int i = 0; i < percents.Count; i++)
                {
                    percents[i] *= ratio;
                    updatedSum += percents[i];
                }
            }

            for (int i = 0; i < OreTypes.Count; i++)
            {
                weights.Add(OreTypes[i], percents[i]);
            }

            weights.Add(ItemType.Stone, 1 - updatedSum);

            return weights;
        }
    }
}
