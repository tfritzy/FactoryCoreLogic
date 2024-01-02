using System.Collections.Generic;

namespace Core
{
    public class MineshaftItem : Item
    {
        public override ItemType Type => ItemType.Mineshaft;
        public override uint MaxStack => 1;
        public override CharacterType? Builds => CharacterType.Mineshaft;
        private const string name = "Mine";
        public override string Name => name;
        public override string? ChemicalFormula => null;

        public MineshaftItem(ulong quantity) : base(quantity) { }
    }
}