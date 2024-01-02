using System.Collections.Generic;

namespace Core
{
    public class SorterItem : Item
    {
        public override ItemType Type => ItemType.Sorter;
        public override uint MaxStack => 1;
        public override CharacterType? Builds => CharacterType.Sorter;
        private const string name = "Sorter";
        public override string Name => name;
        public override string? ChemicalFormula => null;

        public SorterItem(ulong quantity) : base(quantity) { }
    }
}