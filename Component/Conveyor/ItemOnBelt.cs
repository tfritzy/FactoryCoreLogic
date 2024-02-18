using System;

namespace Core
{
    public class ItemOnBelt
    {
        public ulong ItemId;
        public float ProgressMeters;
        public float Min => ProgressMeters - (Item?.Width ?? 0) / 2;
        public float Max => ProgressMeters + (Item?.Width ?? 0) / 2;
        public Item Item
        {
            get
            {
                if (context.World.ItemObjects.ContainsKey(ItemId))
                {
                    return context.World.ItemObjects[ItemId].Item;
                }
                else
                {
                    throw new InvalidOperationException("An item is on a conveyor for which no itemObject exists.");
                }
            }
        }
        private Context context;

        public ItemOnBelt(Context context, ulong id, float progressMeters)
        {
            ItemId = id;
            this.context = context;
            ProgressMeters = progressMeters;
        }

        public ItemOnBelt(Context context, Schema.ItemOnBelt schema)
        {
            this.context = context;
            ProgressMeters = schema.ProgressMeters;
            ItemId = schema.ItemId;
        }

        public Schema.ItemOnBelt ToSchema()
        {
            return new Schema.ItemOnBelt()
            {
                ItemId = ItemId,
                ProgressMeters = ProgressMeters,
            };
        }
    }
}