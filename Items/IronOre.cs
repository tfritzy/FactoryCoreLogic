using System.Collections.Generic;

namespace Core
{
    public class IronOre : Item
    {
        public override ItemType Type => ItemType.IronOre;
        public override int MaxStack => 16;
        public override Dictionary<ItemType, int>? Recipe => null;
        override public float Width => .4f;

        public IronOre(int quantity) : base(quantity) { }
        public IronOre() : base() { }
    }
}