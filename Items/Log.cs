using System.Collections.Generic;

namespace Core
{
    public class Log : Item
    {
        public override ItemType Type => ItemType.Log;
        public override uint MaxStack => 4;
        public override Dictionary<ItemType, uint>? Recipe => null;
        private const string name = "Log";
        public override string Name => name;
        public override string? ChemicalFormula => "C₆H₁₀O₅";

        public Log(ulong quantity) : base(quantity) { }
    }
}