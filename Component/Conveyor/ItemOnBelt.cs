namespace Core
{
    public class ItemOnBelt
    {
        public Item Item;
        public float ProgressMeters;

        public ItemOnBelt(Item item, float progressMeters)
        {
            Item = item;
            ProgressMeters = progressMeters;
        }

        public Schema.ItemOnBelt ToSchema()
        {
            return new Schema.ItemOnBelt()
            {
                Item = Item.ToSchema(),
                ProgressMeters = ProgressMeters,
            };
        }
    }
}