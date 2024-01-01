using Newtonsoft.Json;

namespace Core
{
    public class ItemObjectAdded : Update
    {
        public override UpdateType Type => UpdateType.ItemObjectAdded;
        public ItemObject ItemObject;

        public ItemObjectAdded(ItemObject itemObject)
        {
            this.ItemObject = itemObject;
        }
    }
}