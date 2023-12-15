using System.Collections.Generic;

namespace Core
{
    public class DepotItem : Item
    {
        public override ItemType Type => ItemType.Depot;
        public override int MaxStack => 4;
        public override CharacterType? Builds => CharacterType.Depot;
        private const string name = "Depot";
        public override string Name => name;

        public DepotItem(int quantity) : base(quantity) { }
        public DepotItem() : base() { }
    }
}