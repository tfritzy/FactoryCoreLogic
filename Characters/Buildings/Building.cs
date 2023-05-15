using Core;
using Newtonsoft.Json;

namespace Core
{
    public abstract class Building : Character
    {
        protected Building(Context context) : base(context)
        {
        }

        public override void Destroy()
        {
            this.World.RemoveBuilding(this.GridPosition);
            base.Destroy();
        }
    }
}