namespace Core
{
    public class Stone : Item
    {
        public override ItemType Type => ItemType.Stone;
        public override int MaxStack => 8;
        public Stone(int quantity) : base(quantity) { }
        public Stone() : base() { }
        public override float Width => .4f;
    }
}