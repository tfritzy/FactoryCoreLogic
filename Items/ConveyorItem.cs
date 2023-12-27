using System.Collections.Generic;

namespace Core
{
    public class ConveyorItem : Item
    {
        public override ItemType Type => ItemType.Conveyor;
        public override uint MaxStack => 16;
        public override CharacterType? Builds => CharacterType.Conveyor;
        private const string name = "Conveyor";
        public override string Name => name;
        public override string? ChemicalFormula => null;

        public ConveyorItem(uint quantity) : base(quantity) { }
    }
}