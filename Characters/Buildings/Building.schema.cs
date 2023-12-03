
using Core;
using Newtonsoft.Json;

namespace Schema
{
    public abstract class Building : Character, SchemaOf<Core.Building>
    {
        [JsonProperty("rotation")]
        public HexSide Rotation { get; set; }

        protected override Core.Entity BuildCoreObject(Context context)
        {
            Core.Building building = (Core.Building)base.BuildCoreObject(context);
            building.SetRotation(this.Rotation);
            return building;
        }

        public new Core.Building FromSchema(params object[] context)
        {
            return (Core.Building)this.CreateCore(context);
        }
    }
}