using System.Collections.Generic;
using Core;
using Newtonsoft.Json;
using System;
using System.Linq;

namespace Core
{
    public class Depot : Building
    {
        public override CharacterType Type => CharacterType.Depot;
        public override int Height => 2;
        private static readonly string name = "Depot";
        public override string Name => name;

        public Depot(Context context, int alliance) : base(context, alliance) { }

        protected override void InitComponents()
        {
            SetComponent(new Inventory(this, 4, 6));
            SetComponent(new ItemPort(this));
        }

        public override void ConfigureComponents()
        {
            base.ConfigureComponents();
            ItemPort!.InputSideOffsets = new List<int> { 3 };
            ItemPort!.OutputSideOffsets = new List<int> { 0 };
        }

        public override Schema.Entity BuildSchemaObject()
        {
            return new Schema.Depot();
        }
    }
}
