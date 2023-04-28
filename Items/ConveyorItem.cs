using System.Collections.Generic;

namespace Core
{
    public class ConveyorItem : Item
    {
        public override ItemType Type => ItemType.Conveyor;
        public override int MaxStack => 16;
        public override CharacterType? Builds => CharacterType.Conveyor;

        public ConveyorItem(int quantity) : base(quantity) { }
        public ConveyorItem() : base() { }
    }
}