namespace Core
{
    public class ItemObjectRemoved : Update
    {
        public override UpdateType Type => UpdateType.ItemObjectRemoved;
        public ulong ItemId;

        public ItemObjectRemoved(ulong itemId)
        {
            this.ItemId = itemId;
        }
    }
}