using System;
using System.Collections.Generic;
using Core;
using Newtonsoft.Json;
using static Core.Conveyor;

namespace Schema
{
    public class Conveyor : Character, ISchema<Core.Conveyor>
    {
        public override CharacterType Type => CharacterType.Conveyor;

        public Core.Conveyor FromSchema(object[] context)
        {
            if (context.Length == 0 || context[0] == null || !(context[0] is Entity))
                throw new ArgumentException("Conveyor requires a Context as context[0]");

            Context worldContext = (Context)context[0];

            var component = new Core.Conveyor(worldContext);

            return component;
        }
    }
}
