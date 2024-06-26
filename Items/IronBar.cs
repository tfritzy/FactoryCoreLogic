using System.Collections.Generic;

namespace Core
{
    public class IronBar : Item
    {
        public override ItemType Type => ItemType.IronBar;
        public override ulong MaxStack => 8;
        public override Dictionary<ItemType, uint>? Recipe => null;
        private const string name = "Iron bar";
        public override string Name => name;
        public override string ChemicalFormula => "Fe";

        public IronBar(ulong quantity) : base(quantity) { }
    }
}