namespace Core
{
    public class Wood : Item
    {
        public override ItemType Type => ItemType.Wood;
        public override int MaxStack => 4;
        public Wood(int quantity) : base(quantity) { }
        public Wood() : base() { }
    }
}