using System;
using System.Collections.Generic;
using Core;
using Newtonsoft.Json;
using static Core.Conveyor;

namespace Schema
{
    public class Conveyor : Character
    {
        public override CharacterType Type => CharacterType.Conveyor;

        public override Core.Character FromSchema(params object[] context)
        {
            return this.ToCore(context);
        }
    }
}
