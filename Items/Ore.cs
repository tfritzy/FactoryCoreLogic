namespace Core
{
    public abstract class Ore : Item
    {
        protected Ore(ulong quantity) : base(quantity) { }
        public override uint MaxStack => 200_000_000;
        public override UnitType Units => UnitType.Milligram;
    }
}