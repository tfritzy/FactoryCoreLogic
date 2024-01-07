using Newtonsoft.Json;

namespace Core
{
    public class ItemObjectAdded : Update
    {
        public override UpdateType Type => UpdateType.ItemObjectAdded;
        public Schema.ItemObject ItemObject;

        public ItemObjectAdded(Schema.ItemObject itemObject)
        {
            this.ItemObject = itemObject;
        }
    }
}