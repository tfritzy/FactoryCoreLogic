using Core;
using Newtonsoft.Json;

namespace Schema
{
    public class Sorter : Building
    {
        public override CharacterType Type => CharacterType.Sorter;

        [JsonProperty("filter")]
        public ItemType? Filter { get; set; }

        protected override Core.Entity BuildCoreObject(Context context)
        {
            Core.Sorter core = (Core.Sorter)base.BuildCoreObject(context);
            core.Filter = Filter;
            return core;
        }
    }
}
