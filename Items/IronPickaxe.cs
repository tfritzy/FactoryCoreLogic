using System.Collections.Generic;

namespace Core
{
    public class IronPickaxe : Item
    {
        public override ItemType Type => ItemType.IronPickaxe;
        public override int MaxStack => 1;
        public override Dictionary<ItemType, int> Recipe => recipe;

        public IronPickaxe(int quantity) : base(quantity) { }
        public IronPickaxe() : base() { }

        private static Dictionary<ItemType, int> recipe = new Dictionary<ItemType, int>()
        {
            { ItemType.ToolShaft, 1 },
            { ItemType.IronBar, 2 },
        };
    }
}