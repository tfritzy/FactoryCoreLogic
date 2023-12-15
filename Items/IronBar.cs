using System.Collections.Generic;

namespace Core
{
    public class IronBar : Item
    {
        public override ItemType Type => ItemType.IronBar;
        public override int MaxStack => 8;
        public override Dictionary<ItemType, int>? Recipe => null;
        private const string name = "Iron bar";
        public override string Name => name;

        public IronBar(int quantity) : base(quantity) { }
        public IronBar() : base() { }
    }
}