using System.Collections.Generic;
using Core;
using Newtonsoft.Json;
using System;
using System.Linq;

namespace Core
{
    public class Mineshaft : Building
    {
        public Mine? Component => this.GetComponent<Mine>();
        public override CharacterType Type => CharacterType.Mineshaft;
        public override int Height => 3;

        public Mineshaft(Context context, int alliance) : base(context, alliance) { }

        protected override void InitComponents()
        {
            this.SetComponent(new Mine(this));
            this.SetComponent(new Inventory(this, 4, 4));
        }

        public override Schema.Entity BuildSchemaObject()
        {
            return new Schema.Mineshaft();
        }
    }
}
