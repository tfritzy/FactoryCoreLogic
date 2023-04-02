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

        public override Core.Conveyor FromSchema(params object[] context)
        {
            if (context.Length == 0 || context[0] == null || !(context[0] is Entity))
                throw new ArgumentException("Conveyor requires a Context as context[0]");

            Context worldContext = (Context)context[0];

            var component = new Core.Conveyor(worldContext);

            return component;
        }
    }
}
