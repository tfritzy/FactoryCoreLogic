using System.Collections.Generic;

namespace Core
{
    public class Arrowhead : Item
    {
        public override ItemType Type => ItemType.Arrowhead;
        public override ulong MaxStack => 16;
        public override Dictionary<ItemType, uint> Recipe => recipe;
        private const string name = "Arrowhead";
        public override string Name => name;
        public override string? ChemicalFormula => throw new System.NotImplementedException();

        public Arrowhead(ulong quantity) : base(quantity) { }

        private static Dictionary<ItemType, uint> recipe = new()
        {
            { ItemType.Limestone, 1 },
        };
    }
}