using System.Collections.Generic;

namespace Core
{
    public class Log : Item
    {
        public override ItemType Type => ItemType.Log;
        public override int MaxStack => 4;
        public override Dictionary<ItemType, int>? Recipe => null;
        private const string name = "Log";
        public override string Name => name;

        public Log(int quantity) : base(quantity) { }
        public Log() : base() { }
    }
}