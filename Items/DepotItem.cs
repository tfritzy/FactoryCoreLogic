using System.Collections.Generic;

namespace Core
{
    public class DepotItem : Item
    {
        public override ItemType Type => ItemType.Depot;
        public override uint MaxStack => 4;
        public override CharacterType? Builds => CharacterType.Depot;
        private const string name = "Depot";
        public override string Name => name;
        public override string? ChemicalFormula => null;

        public DepotItem(ulong quantity) : base(quantity) { }
    }
}