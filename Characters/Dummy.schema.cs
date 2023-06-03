using System;
using System.Collections.Generic;
using Core;
using Newtonsoft.Json;

namespace Schema
{
    public class Dummy : Character
    {
        public override CharacterType Type => CharacterType.Dummy;

        protected override Core.Entity BuildCoreObject(Context context)
        {
            return new Core.Dummy(context, this.Alliance);
        }
    }
}
