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
            if (context.Length == 0 || context[0] == null || !(context[0] is Entity))
                throw new ArgumentException("Tree requires a Context as context[0]");

            if (context.Length == 0 || context[0] == null || !(context[0] is Context))
                throw new ArgumentException("Tree requires a Context as context[0]");

            return new Core.Tree((Context)context[0]);
        }

        public override string ToSchema(Core.Character toSerialize)
        {
            throw new NotImplementedException();
        }
    }
}
