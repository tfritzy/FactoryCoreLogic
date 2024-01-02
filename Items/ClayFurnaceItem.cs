using System.Collections.Generic;

namespace Core
{
    public class ClayFurnaceItem : Item
    {
        public override ItemType Type => ItemType.ClayFurnace;
        public override uint MaxStack => 1;
        public override CharacterType? Builds => CharacterType.ClayFurnace;
        private const string name = "Clay furnace";
        public override string Name => name;
        public override string? ChemicalFormula => null;

        public ClayFurnaceItem(ulong quantity) : base(quantity) { }
    }
}