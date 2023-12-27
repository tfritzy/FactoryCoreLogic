namespace Core
{
    public abstract class Ore : Item
    {
        public override bool KgQuantity => true;
        protected Ore(int quantity) : base(quantity) { }
    }
}