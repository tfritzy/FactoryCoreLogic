using System.Collections.Generic;

namespace Core
{
    public class IronPickaxe : Item
    {
        public override ItemType Type => ItemType.IronPickaxe;
        public override uint MaxStack => 1;
        public override Dictionary<ItemType, uint> Recipe => recipe;
        private const string name = "Iron pickaxe";
        public override string Name => name;
        public override string? ChemicalFormula => null;

        public IronPickaxe(uint quantity) : base(quantity) { }

        private static Dictionary<ItemType, uint> recipe = new()
        {
            { ItemType.ToolShaft, 1 },
            { ItemType.IronBar, 2 },
        };
    }
}