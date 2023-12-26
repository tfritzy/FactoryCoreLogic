namespace Core
{
    public class Chalcopyrite : Item
    {
        public override ItemType Type => ItemType.Chalcopyrite;
        public override string Name => "Chalcopyrite";
        public Chalcopyrite(int quantity) : base(quantity) { }
        public Chalcopyrite() { }
    }
}