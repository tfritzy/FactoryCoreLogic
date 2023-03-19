namespace FactoryCore
{
    public class Stone : Item
    {
        public override ItemType Type => ItemType.Stone;
        public override int MaxStack => 8;
    }
}