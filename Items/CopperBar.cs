using System.Collections.Generic;

namespace Core
{
    public class CopperBar : Item
    {
        public override ItemType Type => ItemType.CopperBar;
        public override ulong MaxStack => 8;
        public override Dictionary<ItemType, uint>? Recipe => null;
        private const string name = "Copper bar";
        public override string Name => name;
        public override string ChemicalFormula => "Cu";

        public CopperBar(ulong quantity) : base(quantity) { }
    }
}