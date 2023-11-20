using System.Collections.Generic;

namespace Core
{
    public class MineshaftItem : Item
    {
        public override ItemType Type => ItemType.Mineshaft;
        public override int MaxStack => 1;
        public override CharacterType? Builds => CharacterType.Mineshaft;

        public MineshaftItem(int quantity) : base(quantity) { }
        public MineshaftItem() : base() { }
    }
}