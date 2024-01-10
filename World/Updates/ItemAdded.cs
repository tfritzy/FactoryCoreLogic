using Schema;

namespace Core
{
    public class ItemObjectAdded : Update
    {
        public override UpdateType Type => UpdateType.ItemObjectAdded;
        public SchemaItemObject ItemObject;

        public ItemObjectAdded(SchemaItemObject itemObject)
        {
            this.ItemObject = itemObject;
        }
    }
}