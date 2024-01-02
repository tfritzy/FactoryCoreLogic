using System.Collections.Generic;

namespace Core
{
    public class ToolShaft : Item
    {
        public override ItemType Type => ItemType.ToolShaft;
        public override uint MaxStack => 8;
        public override Dictionary<ItemType, uint> Recipe => recipe;
        private const string name = "Tool shaft";
        public override string Name => name;
        public override string? ChemicalFormula => "C₆H₁₀O₅";

        public ToolShaft(ulong quantity) : base(quantity) { }

        private static readonly Dictionary<ItemType, uint> recipe = new()
        {
            { ItemType.Log, 1 },
        };
    }
}