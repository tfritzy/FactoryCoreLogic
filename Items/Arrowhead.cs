using System.Collections.Generic;

namespace Core
{
    public class Arrowhead : Item
    {
        public override ItemType Type => ItemType.Arrowhead;
        public override int MaxStack => 16;
        public override Dictionary<ItemType, int> Recipe => recipe;
        private const string name = "Arrowhead";
        public override string Name => name;

        public Arrowhead(int quantity) : base(quantity) { }
        public Arrowhead() : base() { }

        private static Dictionary<ItemType, int> recipe = new Dictionary<ItemType, int>()
        {
            { ItemType.Stone, 1 },
        };
    }
}