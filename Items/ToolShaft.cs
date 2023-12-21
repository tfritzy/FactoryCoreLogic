using System.Collections.Generic;

namespace Core
{
    public class ToolShaft : Item
    {
        public override ItemType Type => ItemType.ToolShaft;
        public override int MaxStack => 8;
        public override Dictionary<ItemType, int> Recipe => recipe;
        private const string name = "Tool shaft";
        public override string Name => name;

        public ToolShaft(int quantity) : base(quantity) { }
        public ToolShaft() : base() { }

        private static Dictionary<ItemType, int> recipe = new Dictionary<ItemType, int>()
        {
            { ItemType.Log, 1 },
        };
    }
}