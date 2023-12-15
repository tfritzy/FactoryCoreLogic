namespace Core
{
    public class StoneBrick : Item
    {
        public override ItemType Type => ItemType.StoneBrick;
        public override int MaxStack => 32;
        public StoneBrick(int quantity) : base(quantity) { }
        public StoneBrick() : base() { }
        public override float Width => .4f;
        private const string name = "Stone brick";
        public override string Name => name;
    }
}