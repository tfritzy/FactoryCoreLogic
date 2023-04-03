using System;
using System.Collections.Generic;
using Core;
using Newtonsoft.Json;

namespace Schema
{
    public class Dummy : Character
    {
        public override CharacterType Type => CharacterType.Dummy;

        public override Core.Character FromSchema(object[] context)
        {
            return this.ToCore(context);
        }
    }
}
