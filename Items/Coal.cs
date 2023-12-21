namespace Core
{
    public class Coal : Item
    {
        public override ItemType Type => ItemType.Coal;
        public override int MaxStack => 16;
        public Coal(int quantity) : base(quantity) { }
        public Coal() : base() { }
        override public float Width => .3f;
        private const string name = "Coal";
        public override string Name => name;
    }
}