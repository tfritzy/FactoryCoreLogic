using Newtonsoft.Json;

namespace Core
{
    public class ItemObjectRemoved : Update
    {
        public override UpdateType Type => UpdateType.ItemObjectRemoved;
        public ulong Id;

        public ItemObjectRemoved(ulong id)
        {
            Id = id;
        }
    }
}