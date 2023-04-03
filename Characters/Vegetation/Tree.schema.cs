using System;
using Core;
using Newtonsoft.Json;

namespace Schema
{
    public class Tree : Character
    {
        public override CharacterType Type => CharacterType.Tree;

        public override Core.Character FromSchema(object[] context)
        {
            return this.ToCore(context);
        }
    }
}
