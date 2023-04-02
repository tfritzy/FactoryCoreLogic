using System;
using System.Collections.Generic;
using Core;
using Newtonsoft.Json;

namespace Schema
{
    public class Dummy : Character, Schema<Core.Dummy>
    {
        public override CharacterType Type => CharacterType.Dummy;

        public Core.Dummy FromSchema(object[] context)
        {
            if (context.Length == 0 || context[0] == null || !(context[0] is Entity))
                throw new ArgumentException("Dummy requires a Context as context[0]");

            Context worldContext = (Context)context[0];

            var component = new Core.Dummy(worldContext);

            return component;
        }
    }
}
