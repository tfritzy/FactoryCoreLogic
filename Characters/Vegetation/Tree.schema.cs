using System;
using Core;
using Newtonsoft.Json;

namespace Schema
{
    public class Tree : Vegetation
    {
        public override VegetationType Type => VegetationType.Tree;

        protected override Core.Vegetation BuildCoreObject(Context context)
        {
            return new Core.Tree(context);
        }
    }
}
